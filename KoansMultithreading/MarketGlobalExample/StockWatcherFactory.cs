using KoansMultithreading.GlobalTools.Contract;
using KoansMultithreading.MarketGlobalExample.MarketCompute;

namespace TestStructure
{
    public class StockWatcherFactory
    {
        private readonly IProducerFactory<StockMarketData> _producerFactory;

        public StockWatcherFactory(IProducerFactory<StockMarketData> producerFactory) => _producerFactory = producerFactory;

        public (IStockWatcher, ComputeMeanOnLineState, ComputeVarianceOnLineState) CreateInstance(string name, double initValue, double volatility)
        {
            ComputeMeanOnLine computeMean = new ComputeMeanOnLine();
            ComputeVarianceOnline computeVariance = new ComputeVarianceOnline();
            ComputeMeanOnLineState meanState = null;
            ComputeVarianceOnLineState varianceState = null;
            var stockWatcher = new StockWatcher(new RandomStockWalker(initValue, volatility), _producerFactory.CreateInstance(), name, initValue);
            // -----------------------------------------------------------------------------------------------------------------------------------
            // Set state compute for testing purpose.
            // -----------------------------------------------------------------------------------------------------------------------------------
            // local function to trigger variance computing after mean has been computed.
            void triggerComputeVariance(ComputeMeanOnLineState state)
            {
                meanState = state;
                computeVariance.Compute(state.LastAddedValue, state.Mean);
            }
            // register handler on event.
            // - trigger variance computing
            computeMean.OnChangeState += triggerComputeVariance;
            // - trigger variance state collecting
            computeVariance.OnChangeState += (ComputeVarianceOnLineState state) => varianceState = state;
            // - trigger mean computing each time a new value have been received.
            stockWatcher.OnTick += (newValue) => computeMean.Compute(newValue);
            // -----------------------------------------------------------------------------------------------------------------------------------
            // Return IStockWatcher and the two state that will be used as reference during the testing phasis.
            // -----------------------------------------------------------------------------------------------------------------------------------
            return (stockWatcher, meanState, varianceState);
        }
    }
}