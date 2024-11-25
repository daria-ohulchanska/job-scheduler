using JobScheduler.Core.Identity;
using JobScheduler.Core.Messaging;
using JobScheduler.Core.Services;
using JobScheduler.Data;
using JobScheduler.Data.Contexts;
using JobScheduler.Data.Repositories;
using JobScheduler.Services.Scheduler;
using JobScheduler.Shared.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sheduler",
                Version = "v1"
            });
        });

        var connectionString = builder.Configuration.GetConnectionString("JobSchedulerDb");

        builder.Services.AddDbContext<ApplicationDbContext>(options => 
            options.UseNpgsql(connectionString));

        var queueSettings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();

        builder.Services.AddSingleton<IConnection>(_ =>
        {
            var factory = new ConnectionFactory
            {
                HostName = queueSettings.HostName,
                UserName = queueSettings.UserName,
                Password = queueSettings.Password
            };
            
            return factory.CreateConnection();
        });

        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            
            options.User.RequireUniqueEmail = true;
        });
        
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.Configure<ConcurrentSchedulerSettings>(builder.Configuration.GetSection("ConcurrentScheduler"));
        builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("Authentication"));
        
        builder.Services.AddSingleton(typeof(IMessageQueuePublisher), typeof(RabbitMqPublisher));
        builder.Services.AddSingleton(typeof(IScheduler), typeof(ConcurrentScheduler));
        builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));
        builder.Services.AddScoped(typeof(IJobRepository), typeof(JobRepository));
        builder.Services.AddScoped(typeof(IJobHistoryRepository), typeof(JobStatusHistoryRepository));
        builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
        builder.Services.AddScoped(typeof(IIdentityService), typeof(IdentityService));
        builder.Services.AddScoped(typeof(ITokenService), typeof(TokenService));
        
        var authSettings = builder.Configuration.GetSection("Authentication").Get<AuthenticationSettings>();
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidAudience = authSettings.Audience,
                    IssuerSigningKey = authSettings.GetKey(),
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        builder.Services.AddAuthorization();

        builder.Services.AddRazorPages();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(configure =>
            {
                configure.SwaggerEndpoint("/swagger/v1/swagger.json", "Scheduler V1");
            });
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapRazorPages();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        
        app.MapControllers()
            .RequireAuthorization();

        app.MapSwagger()
            .RequireAuthorization("Admin");

        app.Run();
    }
}