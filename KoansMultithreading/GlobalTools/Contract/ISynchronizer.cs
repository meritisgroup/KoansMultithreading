using System;

namespace KoansMultithreading.GlobalTools.Contract
{
    public interface ISynchronizer : IDisposable
    {
        void Signal();
        void Wait();
    }
}
