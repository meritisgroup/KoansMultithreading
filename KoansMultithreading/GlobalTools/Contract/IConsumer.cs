using System;
using System.Threading.Tasks;

namespace KoansMultithreading.GlobalTools.Contract
{
    public interface IConsumer
    {
        Task StartConsume();

        void StopConsume();
    }

    public interface IConsumer<out T> : IConsumer
    {
        event Action<T> OnConsume;
    }
}