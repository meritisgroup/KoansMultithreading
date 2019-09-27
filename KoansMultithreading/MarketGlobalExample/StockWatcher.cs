using KoansMultithreading.GlobalTools.Contract;
using System;
using System.Diagnostics;
using System.Threading;

namespace TestStructure
{
    public class StockWatcher : IStockWatcher
    {
        #region Private fields

        private readonly IStockUpdater _updater;
        private readonly Timer _timer;
        private readonly IProducer<StockMarketData> _mdProducer;
        private Stopwatch _watch;
        private long _lastCheck;
        private StockMarketData _current;

        #endregion Private fields

        #region implementaiton of IStockWatcher

        public string Name { get; }

        public double CurrentValue => _updater.Value;

        public event Action<double> OnTick;

        #endregion implementaiton of IStockWatcher

        public StockWatcher(IStockUpdater updater, IProducer<StockMarketData> mdProducer, string name, double initValue)
        {
            _timer = new Timer(new TimerCallback(HandleTick), null, 500, 500);
            _watch = new Stopwatch();
            _watch.Start();
            _updater = updater;
            _mdProducer = mdProducer;
            _current = new StockMarketData(name, initValue, initValue, initValue);
        }

        /// <summary>
        /// CallBack for timer
        /// </summary>
        /// <param name="arg"> Unused argument because we must not transfert state to callback during the callback.</param>
        private void HandleTick(object arg)
        {
            var nextLastCheck = _watch.ElapsedMilliseconds;
            var newValue = _updater.UpdateValue(nextLastCheck - _lastCheck);
            _lastCheck = nextLastCheck;
            _current = _current.UpDateValue(newValue);
            _mdProducer.Post(_current);
        }

        #region IDisposable implementation

        public void Dispose()
        {
            _watch.Stop();
            _watch = null;
            _timer.Dispose();
        }

        #endregion IDisposable implementation
    }
}