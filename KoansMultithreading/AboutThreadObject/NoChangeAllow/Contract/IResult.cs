using System;

namespace KoansMultithreading.AboutThreadObject.NoChangeAllow.Contract
{
    public interface IResult
    {
        bool HasError { get; }
        Exception InternalError { get; }
    }

    public interface IResult<T> : IResult
    {
        T Value { get; }
    }
}