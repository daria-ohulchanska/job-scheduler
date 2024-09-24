using JobScheduler.Core.Services;
using JobScheduler.Data.Contexts;
using JobScheduler.Data.Repositories;
using JobScheduler.Services.Scheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

        builder.Services.AddScoped(typeof(IJobRepository), typeof(JobRepository));
        builder.Services.AddScoped(typeof(IJobHistoryRepository), typeof(JobStatusHistoryRepository));

        builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));
        builder.Services.AddScoped<IScheduler>(sp =>
        {
            var jobRepository = sp.GetRequiredService<IJobRepository>();
            var jobHistoryRepository = sp.GetRequiredService<IJobHistoryRepository>();

            return new ConcurrentScheduler(jobRepository, jobHistoryRepository, capacity: 2);
        });

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

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllers();

        app.Run();
    }
}