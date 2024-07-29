namespace JobScheduler.Models
{
    public class Robot : IRobot
    {
        public bool IsActive { get; private set; }

        public int Id { get; }

        public Robot(int id)
        {
            Id = id;
        }

        public void Start() =>
            IsActive = true;

        public void Stop() =>
            IsActive = false;
    }
}
