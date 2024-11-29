using JobScheduler.Shared.Constants;
using JobScheduler.Shared.Enums;

namespace JobScheduler.Shared.Extensions
{
    public static class JobExtensions
    {
        public static TimeSpan Duration(this JobType jobType) => 
            JobDuration.CalculateBy(jobType);
    }
}
