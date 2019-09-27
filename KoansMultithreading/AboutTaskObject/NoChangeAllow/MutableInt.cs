namespace KoansMultithreading.AboutTaskObject.NoChangeAllow
{
    public class MutableInt
    {
        private static readonly object _locker = new object();
        private int _value;

        public int Value
        {
            get { return _value; }
        }

        public MutableInt(int value) => _value = value;

        public void AtomicSum(int arg)
        {
            lock (_locker)
            {
                _value += arg;
            }
        }
    }
}