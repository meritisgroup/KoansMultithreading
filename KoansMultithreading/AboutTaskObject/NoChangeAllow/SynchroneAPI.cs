using System;
using System.Threading;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow
{
    public class SynchroneAPI
    {
        public int GetAnswer()
        {
            Thread.Sleep(1000);
            return 42;
        }

        public int TryGetAnswer()
        {
            Thread.Sleep(1000);
            throw new TimeoutException("No answer found, need to read the good book....");
        }
    }
}