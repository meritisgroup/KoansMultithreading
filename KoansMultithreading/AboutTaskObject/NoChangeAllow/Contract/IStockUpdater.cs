namespace KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract
{
    public interface IStockUpdater
    {
        double Value { get; }

        double UpdateValue(long timeInMillisecond);
    }
}