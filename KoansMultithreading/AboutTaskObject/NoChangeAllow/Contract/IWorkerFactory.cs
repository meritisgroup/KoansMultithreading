using System;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract
{
    public interface IWorkerFactory<T>
    {
        Func<T> CreateWorker();
    }
}