namespace KoansMultithreading.GlobalTools.Contract
{
    public interface IConsumerFactory
    {
        IConsumer CreateInstance();
    }

    public interface IConsumerFactory<out T> : IConsumerFactory
    {
        new IConsumer<T> CreateInstance();
    }
}