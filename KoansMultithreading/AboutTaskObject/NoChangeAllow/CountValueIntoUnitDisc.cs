using KoansMultithreading.GlobalTools.Contract;
using KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract;
using KoansMultithreading.GlobalTools.NoChangeAllow;
using System;
using System.Threading.Tasks;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow
{
    /// <summary>
    /// Count the value of point instance that are in the unit circle center on (0, 0).
    /// It is also a proxy on a IConsumer instance.
    /// </summary>
    public sealed class CountValueIntoUnitDisc : IConsumer<Point>, ICountValueIntoUnitDisc, IDisposable
    {
        #region Private fields

        private readonly IConsumer<Point> _technicalConsumer;
        private readonly object _locker = new object();

        #endregion Private fields

        #region Constructor

        public CountValueIntoUnitDisc(IConsumer<Point> technicalConsumer)
        {
            _technicalConsumer = technicalConsumer;
            OnConsume += HandleNewPoint;
        }

        #endregion Constructor

        #region Implementation of IConsumer<Point>

        public event Action<Point> OnConsume
        {
            add
            {
                _technicalConsumer.OnConsume += value;
            }
            remove
            {
                _technicalConsumer.OnConsume -= value;
            }
        }

        #region Implementation of IConsumer

        public Task StartConsume() => _technicalConsumer.StartConsume();

        public void StopConsume() => _technicalConsumer.StopConsume();

        #endregion Implementation of IConsumer

        #endregion Implementation of IConsumer<Point>

        #region Implementation of ICountValueIntoUnitDisc

        /// <summary>
        /// Total number of point received.
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Number of point inside the unit circle.
        /// </summary>
        public int CountIn { get; private set; }

        public double ProbaInUnitCircle()
        {
            lock (_locker)
            {
                return (double)CountIn / TotalCount;
            }
        }

        public void HandleNewPoint(Point point)
        {
            lock (_locker)
            {
                TotalCount++;
                if (point.SquareNorm() < 1) CountIn++;
            }
        }

        #endregion Implementation of ICountValueIntoUnitDisc

        #region Implementation of IDisposable

        public void Dispose() => OnConsume -= HandleNewPoint;

        #endregion Implementation of IDisposable
    }
}