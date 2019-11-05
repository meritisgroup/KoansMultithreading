using System;
using System.Threading;
using System.Threading.Tasks;
using KoansMultithreading.AboutThreadSynchronizationObject.NoChangeAllow;

namespace KoansMultithreading.AboutThreadSynchronizationObject.CanBeChanged
{
    public class AutoResetSynchronizedRunner<T> : IDisposable, ISynchronizedRunner<T>
    {
        private readonly AutoResetEvent _are = new AutoResetEvent(false);
        private readonly Action<T> _job;
        private bool _stop;
        private T _data;

        public AutoResetSynchronizedRunner(Action<T> job)
        {
            _job = job;
            Task.Run(Run);
        }

        public void Dispose() => _are.Close();

        public void SetData(T data)
        {
            _data = data;
            // notify that new data is available
            _are.Set();
        }

        public void Stop() => _stop = true;

        // This runs in separate thread and waits for d to be set to a new value
        private void Run()
        {
            while (!_stop)
            {
                // waits for new data to process
                _are.WaitOne();
                _job(_data);
            }
        }
    }
}