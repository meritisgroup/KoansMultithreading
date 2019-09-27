using KoansMultithreading.AboutThreadObject.NoChangeAllow.Contract;
using System;

namespace KoansMultithreading
{
    public interface IJob<T>
    {
        event Action OnEndOfJob;

        IJob<T> StartWith(Func<T> runner);

        IJob<T> StartWith(Func<T> runner, Func<Exception, Exception> onError);

        void Wait();

        IJob<T> Run();

        IJob<TOut> Then<TOut>(Func<IResult<T>, TOut> nextToBeRunned);

        IJob<TOut> Then<TOut>(Func<IResult<T>, TOut> nextToBeRunned, Func<Exception, Exception> onError);

        IResult<T> Result { get; }
    }
}