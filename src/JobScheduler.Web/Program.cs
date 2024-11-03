﻿using JobScheduler.Core.BackgroundServices;
using JobScheduler.Core.Messaging;
using JobScheduler.Core.Services;
using JobScheduler.Data;
using JobScheduler.Data.Contexts;
using JobScheduler.Services.Scheduler;
using JobScheduler.Shared.Configurations;
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

        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.Configure<ConcurrentSchedulerSettings>(builder.Configuration.GetSection("ConcurrentScheduler"));

        builder.Services.AddSingleton<IMessageQueuePublisher, RabbitMqPublisher>();
        builder.Services.AddHostedService<JobStatusProcessor>();

        builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));
        builder.Services.AddSingleton(typeof(IScheduler), typeof(ConcurrentScheduler));
        builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

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

        app.MapIdentityApi<UserEntity>();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllers();

        app.Run();
    }
}