using System.Threading;

namespace KoansMultithreading.AboutThreadSynchronizationObject.NoChangeAllow
{
    public interface ISynchronizationContextFactory
    {
        SynchronizationContext Create();
    }
}