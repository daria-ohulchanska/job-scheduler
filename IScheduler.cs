namespace JodScheduler
{
    public interface IScheduler
    {
        void Schedule(IJob job);
        void Stop();
    }
}
