using KoansMultithreading.AboutThreadSynchronizationObject.NoChangeAllow;
using System;

namespace KoansMultithreading.AboutThreadSynchronizationObject.CanBeChanged
{
    public class SynchronizedRunnerFactory<T>
    {
        public ISynchronizedRunner<T> Create(SynchronisationTestCase testCase, Action<T> job)
        {
            return testCase switch
            {
                SynchronisationTestCase.ManualReset => null,
                _ => new AutoResetSynchronizedRunner<T>(job),
            };
        }
    }
}
