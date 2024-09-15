namespace JobScheduler.Models
{
    public interface IJob
    {
        string Name { get; }
        string Description { get; }

        void Run();
    }
}