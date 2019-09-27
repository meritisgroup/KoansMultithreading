using MathNet.Numerics.Random;
using System;

namespace TestStructure
{
    public class RandomStockWalker : IStockUpdater
    {
        private readonly Random generator;
        public double Volatility { get; }

        public double Value { get; private set; }

        public RandomStockWalker(double initialValue, double volatility)
        {
            Value = initialValue;
            Volatility = volatility;
            generator = new Xorshift(DateTime.UtcNow.Millisecond);
        }

        public double UpdateValue(long timeInMillisecond)
        {
            var variation = Math.Sqrt(((double)timeInMillisecond) / 1000.0) * Volatility;
            Value = generator.Next(-100, 100) < 0 ? Value - variation : Value + variation;
            return Value;
        }
    }
}