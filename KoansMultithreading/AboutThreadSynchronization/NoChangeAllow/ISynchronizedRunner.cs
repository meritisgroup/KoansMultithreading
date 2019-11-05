using System;

namespace KoansMultithreading.AboutThreadSynchronizationObject.NoChangeAllow
{
    public interface ISynchronizedRunner<T> : IDisposable
    {
        void SetData(T data);
        void Stop();
    }
}