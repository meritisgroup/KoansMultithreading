using System;

namespace KoansMultithreading
{
    public static class LambdaExtension
    {
        public static Func<TIn, TOut> Then<TIn, TInterm, TOut>(this Func<TIn, TInterm> firstFun, Func<TInterm, TOut> secondFun)
            => (arg) => secondFun(firstFun(arg));

        public static Action<TIn> Then<TIn, TOut>(this Func<TIn, TOut> firstFun, Action<TOut> secondFun)
            => (arg) => secondFun(firstFun(arg));

        public static Action Then(this Action firstStep, Action secondStep) => () => { firstStep(); secondStep(); };

        public static Action<TIn> Then<TIn>(this Action<TIn> firstStep, Action secondStep) => (arg) => { firstStep(arg); secondStep(); };
    }
}