using System;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow
{
    public static class AboutTaskHelper
    {
        public static Action<int> GetSumAction(MutableInt accumulator) => (arg) => accumulator.AtomicSum(arg);
    }
}