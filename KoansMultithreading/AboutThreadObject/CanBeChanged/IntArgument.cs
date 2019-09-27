using System.Threading;
using KoansMultithreading.AboutThreadObject.NoChangeAllow.Contract;

namespace KoansMultithreading.AboutThreadObject.CanBeChanged
{
    public class IntArgument : IArgument<int>
    {
        private int _value;

        public IntArgument(int value) => _value = value;

        public int Cast() => _value;

        public static IntArgument operator +(IntArgument left, IntArgument rigth)
        {
            left._value = left.Cast() + rigth.Cast();
            return left;
        }

        public static ParameterizedThreadStart GetSumParameterizedThreadStart(IntArgument accumulator)

           => new ParameterizedThreadStart((arg) => accumulator += arg as IntArgument);
    }
}