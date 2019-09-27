using System;
using System.Threading;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract
{
    public interface ICancellableWorkerFactory<T>
    {
        Func<T> CreateWorker(CancellationToken cancellationToken);
    }
}