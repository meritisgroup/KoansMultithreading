namespace TestStructure
{
    public class StockMarketData
    {
        public string Name { get; }

        public double Min { get; }

        public double Max { get; }

        public double Current { get; }

        public StockMarketData(string name, double current, double min, double max)
        {
            Name = name;
            Current = current;
            Min = min;
            Max = max;
        }

        public StockMarketData UpDateValue(double newValue)
        {
            if (newValue < Min)
            {
                return new StockMarketData(Name, newValue, newValue, Max);
            }
            else if (newValue > Max)
            {
                return new StockMarketData(Name, newValue, Min, newValue);
            }
            return new StockMarketData(Name, newValue, Min, Max);
        }
    }
}