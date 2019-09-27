namespace KoansMultithreading.AboutTaskObject.NoChangeAllow
{
    public class MutableBool
    {
        private static readonly object _locker = new object();
        private bool _value;

        public bool Value
        {
            get { return _value; }
        }

        public MutableBool(bool initValue = false) => _value = initValue;

        public void SetFalseAtomic() => SetvalueAtomic(false);

        public void SetTrueAtomic() => SetvalueAtomic(true);

        private void SetvalueAtomic(bool newValue)
        {
            lock (_locker)
            {
                _value = newValue;
            }
        }
    }
}