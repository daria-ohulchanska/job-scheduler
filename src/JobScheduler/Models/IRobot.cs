namespace JobScheduler.Models
{
    public interface IRobot
    {
        public int Id { get; }
        public bool IsActive { get; }
        void Start();
        void Stop();
    }
}
