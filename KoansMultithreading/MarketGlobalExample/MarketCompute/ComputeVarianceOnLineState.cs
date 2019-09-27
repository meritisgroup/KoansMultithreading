namespace KoansMultithreading.MarketGlobalExample.MarketCompute
{
    public class ComputeVarianceOnLineState
    {
        public double LastAddedValue { get; }
        public double Mean { get; }
        public double NewsquaredSumOfDifference { get; }
        public double Variance { get; }
        public int Count { get; }

        public ComputeVarianceOnLineState(double newValue, double mean, double newsquaredSumOfDifference, double standardVariation)
        {
            Mean = mean;
            Variance = standardVariation;
            NewsquaredSumOfDifference = newsquaredSumOfDifference;
            Count = 0;
        }

        public ComputeVarianceOnLineState(double newValue, double mean, double newsquaredSumOfDifference, double standardVariation, int count)
        {
            Mean = mean;
            NewsquaredSumOfDifference = newsquaredSumOfDifference;
            Variance = standardVariation;
            Count = count;
        }
    }
}