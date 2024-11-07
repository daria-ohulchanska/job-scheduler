using JobScheduler.Core.Messaging;
using JobScheduler.Core.Services;
using JobScheduler.Data;
using JobScheduler.Data.Contexts;
using JobScheduler.Data.Entities;
using JobScheduler.Data.Repositories;
using JobScheduler.Services.Scheduler;
using JobScheduler.Shared.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        builder.Services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory
            {
                HostName = queueSettings.HostName,
                UserName = queueSettings.UserName,
                Password = queueSettings.Password
            };
            
            return factory.CreateConnection();
        });

        builder.Services.AddRazorPages();
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.Configure<ConcurrentSchedulerSettings>(builder.Configuration.GetSection("ConcurrentScheduler"));

        builder.Services.AddSingleton<IMessageQueuePublisher, RabbitMqPublisher>();

        builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));
        builder.Services.AddSingleton(typeof(IScheduler), typeof(ConcurrentScheduler));
        builder.Services.AddScoped(typeof(IJobRepository), typeof(JobRepository));
        builder.Services.AddScoped(typeof(IJobHistoryRepository), typeof(JobStatusHistoryRepository));
        builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

        builder.Services.AddAuthentication();
        
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true; 
                options.Password.RequiredLength = 8; 
                options.Password.RequireNonAlphanumeric = false; 
                options.Password.RequireDigit = false; 
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

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

        app.MapIdentityApi<IdentityUser>();

        app.MapRazorPages();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        
        app.MapControllers()
            .RequireAuthorization();

        app.Run();
    }
}