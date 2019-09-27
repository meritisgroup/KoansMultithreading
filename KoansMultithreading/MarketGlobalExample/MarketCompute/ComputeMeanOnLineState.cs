namespace KoansMultithreading.MarketGlobalExample.MarketCompute
{
    public class ComputeMeanOnLineState
    {
        public double LastAddedValue { get; }
        public int Count { get; }
        public double Mean { get; }

        public ComputeMeanOnLineState(double newValue, double mean)
        {
            Mean = mean;
            Count = 0;
        }

        public ComputeMeanOnLineState(double newValue, double mean, int count)
        {
            Mean = mean;
            Count = count;
        }
    }
}
