using JobScheduler.Shared.Enums;

namespace JobScheduler.Shared.Constants
{
    public static class JobDuration
    {
        private const int Fast = 1;
        private const int Medium = 2;
        private const int Slow = 3;

        public static TimeSpan CalculateBy(JobType jobType)
        {
            var minutes =  jobType switch
            {
                JobType.Long => Slow,
                JobType.Normal => Medium,
                JobType.Short => Fast,
                _ => 0,
            };

            return TimeSpan.FromMinutes(minutes);
        }
    }
}
