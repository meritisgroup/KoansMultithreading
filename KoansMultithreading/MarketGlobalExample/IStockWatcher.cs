using System;

namespace TestStructure
{
    public interface IStockWatcher : IDisposable
    {
        double CurrentValue { get; }
        string Name { get; }

        event Action<double> OnTick;
    }
}