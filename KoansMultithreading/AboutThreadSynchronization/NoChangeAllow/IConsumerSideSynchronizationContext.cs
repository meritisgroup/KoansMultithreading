namespace KoansMultithreading.AboutThreadSynchronizationObject.NoChangeAllow
{
    public interface IConsumerSideSynchronizationContext
    {
        void Complete();

        void RunOnCurrentThread();
    }
}