namespace JobScheduler.Models
{
    public interface IJob
    {
        Guid Id { get; }
        Guid UserId { get; }
        string Name { get; }
        string Description { get; }

        Task Run();
    }
}