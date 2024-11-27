namespace JobScheduler.Models
{
    public interface IJob
    {
        Guid Id { get; }
        string UserId { get; }
        string Name { get; }
        string Description { get; }

        Task Run();
    }
}