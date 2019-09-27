using System;

namespace KoansMultithreading.MarketGlobalExample.MarketCompute
{
    public interface IComputable<TInOUt, TState>
    {
        event Action<TState> OnChangeState;

        TInOUt Compute(TInOUt newvalue);
    }

    public interface IComputable<TIn1, TIn2, TOut, TState>
    {
        event Action<TState> OnChangeState;

        TOut Compute(TIn1 newIn1Value, TIn2 newIn2Value);
    }
}