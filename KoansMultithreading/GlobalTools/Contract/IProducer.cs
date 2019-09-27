namespace KoansMultithreading.GlobalTools.Contract
{
    public interface IProducer<in T>
    {
        void Post(T producedValue);
    }
}