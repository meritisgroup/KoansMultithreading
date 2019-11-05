using System;
using System.Threading;
using System.Threading.Tasks;

namespace KoansMultithreading.AboutThreadSynchronizationObject.NoChangeAllow
{
    /// <summary>
    /// Provides a runner that run async function using a specific SynchronizationContext object.
    /// </summary>
    public class TaskRunner
    {
        private readonly SynchronizationContext _synchronizationContext;
        private readonly Action<Task> _continuationOnTaskEnd;
        private readonly Action<Task> _afterExecution;

        public TaskRunner(SynchronizationContext synchronizationContext, Action<Task> continuationOnTaskEnd, Action<Task> afterExecution)
        {
            _synchronizationContext = synchronizationContext;
            _continuationOnTaskEnd = continuationOnTaskEnd;
            _afterExecution = afterExecution;
        }

        /// <summary>Runs the specified asynchronous function.</summary>
        /// <param name="func">The asynchronous function to execute.</param>
        public void Run(Func<Task> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            var prevCtx = SynchronizationContext.Current;
            try
            {
                // Establish the new context
                SynchronizationContext.SetSynchronizationContext(_synchronizationContext);

                // Invoke the function and alert the context to when it completes
                var t = func();
                if (t == null) throw new InvalidOperationException("No task provided.");
                if (_continuationOnTaskEnd != null)
                {
                    t.ContinueWith(_continuationOnTaskEnd, TaskScheduler.Default);
                }

                _afterExecution?.Invoke(t);
            }
            finally { SynchronizationContext.SetSynchronizationContext(prevCtx); }
        }
    }
}