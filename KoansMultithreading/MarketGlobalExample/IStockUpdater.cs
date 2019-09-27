namespace TestStructure
{
    public interface IStockUpdater
    {
        double Value { get; }

        double UpdateValue(long timeInMillisecond);
    }
}