namespace KoansMultithreading.GlobalTools.Contract
{
    public interface IProducerFactory<in T>
    {
        IProducer<T> CreateInstance();
    }
}