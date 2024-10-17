using JobScheduler.Core.Messaging;
using JobScheduler.Data.Entities;
using JobScheduler.Data.Repositories;
using JobScheduler.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace JobScheduler.Core.BackgroundServices
{
    public class JobStatusProcessor : RabbitMqConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public JobStatusProcessor(
            IServiceScopeFactory scopeFactory,
            IConnection connection,
            IOptions<RabbitMqSettings> settings) 
            : base(connection, settings)
        {
            _scopeFactory = scopeFactory;
        }

        public async override Task ProcessMessage(string message)
        {
            var jobHistory = JsonConvert.DeserializeObject<JobHistoryEntity>(message);
            if (jobHistory == null)
                return;

            using var scope = _scopeFactory.CreateScope();

            var jobHistoryRepository = scope.ServiceProvider.GetRequiredService<IJobHistoryRepository>();
            var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();

            await jobHistoryRepository.AddAsync(jobHistory);
            await jobRepository.UpdateAsync(jobHistory.JobId, jobHistory.Status);
        }
    }
}

