using KoansMultithreading.GlobalTools.Contract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KoansMultithreading.AboutThreadSynchronizationObject.NoChangeAllow
{
    public abstract class AbstractSynchronizedRunner<T> : ISynchronizedRunner<T>
    {
        private readonly Action<T> _job;
        private bool _stop;

        protected ISynchronizer RunnerAwaiter { get; }
        protected ISynchronizer SenderAwaiter { get; }
        protected T _data;

        protected AbstractSynchronizedRunner(Action<T> job, ISynchronizer areRunnerAwaiter, ISynchronizer areSenderAwaiter)
        {
            _job = job;
            RunnerAwaiter = areRunnerAwaiter;
            SenderAwaiter = areSenderAwaiter;
            Task.Run(Run);
        }

        public abstract void SetData(T data);

        public void Stop() => _stop = true;

        public void Dispose()
        {
            Stop();
            RunnerAwaiter.Dispose();
            SenderAwaiter.Dispose();
        }

        // This runs in separate thread and waits for d to be set to a new value
        private void Run()
        {
            while (!_stop)
            {
                // waits for new data to process
                RunnerAwaiter.Wait();
                _job(_data);
                SenderAwaiter.Signal();
            }
        }
    }
}