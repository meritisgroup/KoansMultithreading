using System;
using System.Threading;

namespace KoansMultithreading.MarketGlobalExample.MarketCompute
{
    public class ComputeVarianceOnline : IComputable<double, double, double, ComputeVarianceOnLineState>
    {
        private Func<ComputeVarianceOnLineState, double, double, ComputeVarianceOnLineState> _currentCompute;
        private ComputeVarianceOnLineState _currentState;

        public event Action<ComputeVarianceOnLineState> OnChangeState;

        public ComputeVarianceOnline() => _currentCompute = InitComputeMeanIteratively;

        public double Compute(double newValue, double newMean)
        {
            _currentState = _currentCompute(_currentState, newValue, newMean);
            HandleStateChange();
            return _currentState.Variance;
        }

        private ComputeVarianceOnLineState InitComputeMeanIteratively(ComputeVarianceOnLineState currentState, double newValue, double newMean)
        {
            _currentCompute = ComputeMeanIteratively;
            return new ComputeVarianceOnLineState(newValue, newMean, 0, 0, 1);
        }

        private ComputeVarianceOnLineState ComputeMeanIteratively(ComputeVarianceOnLineState currentState, double newValue, double newMean)
        {
            var newsquaredSumOfDifference = _currentState.Variance + (newValue - _currentState.Mean) * (newValue - newMean);
            var newVariance = Math.Sqrt(newsquaredSumOfDifference / _currentState.Count);
            return new ComputeVarianceOnLineState(newValue, newMean, newsquaredSumOfDifference, newVariance, _currentState.Count + 1);
        }

        private void HandleStateChange()
        {
            Volatile.Read(ref OnChangeState)?.Invoke(_currentState);
        }
    }
}