using System;
using System.Threading;

namespace KoansMultithreading.MarketGlobalExample.MarketCompute
{
    public class ComputeMeanOnLine : IComputable<double, ComputeMeanOnLineState>
    {
        private Func<ComputeMeanOnLineState, double, ComputeMeanOnLineState> _currentCompute;

        private ComputeMeanOnLineState _currentState;

        public event Action<ComputeMeanOnLineState> OnChangeState;

        public ComputeMeanOnLine() => _currentCompute = InitComputeMeanIteratively;

        public double Compute(double newvalue)
        {
            _currentState = _currentCompute(_currentState, newvalue);
            HandleStateChange();
            return _currentState.Mean;
        }

        private ComputeMeanOnLineState InitComputeMeanIteratively(ComputeMeanOnLineState currentState, double newValue)
        {
            _currentCompute = ComputeMeanIteratively;
            return new ComputeMeanOnLineState(0, newValue, 1);
        }

        private ComputeMeanOnLineState ComputeMeanIteratively(ComputeMeanOnLineState currentState, double newValue)
        {
            var newCount = currentState.Count + 1;
            var mean = currentState.Mean + (1.0 / currentState.Count) * (newValue - currentState.Mean);
            return new ComputeMeanOnLineState(newValue, mean, newCount);
        }

        private void HandleStateChange()
        {
            Volatile.Read(ref OnChangeState)?.Invoke(_currentState);
        }
    }
}