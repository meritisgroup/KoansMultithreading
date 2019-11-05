using System;
using System.Threading;

namespace KoansMultithreading.AboutThreadSynchronizationObject.CanBeChanged
{
    public sealed class SingleThreadSynchronizationContext : SynchronizationContext
    {
        /// <summary>
        /// To be implemented
        /// </summary>
        public sealed override void Post(SendOrPostCallback d, object state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// To be implemented ... but not necessarily supported (why ?)
        /// </summary>
        public sealed override void Send(SendOrPostCallback d, object state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// To be implemented
        /// Use to continue pumping the action to be done and kept into the synchronization context.
        /// </summary>
        public void RunOnCurrentThread()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// To be implemented
        /// To Complete the operation once all the action into synchronization context have been completed.
        /// </summary>
        public void Complete()
        {
            throw new NotImplementedException();
        }
    }
}