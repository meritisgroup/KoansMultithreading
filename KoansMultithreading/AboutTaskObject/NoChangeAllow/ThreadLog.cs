using System.Threading;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow
{
    public class ThreadLog<T>
    {
        public T Result { get; set; }

        public int ThreadId { get; private set; }

        public int TaskId { get; set; }

        public void SetThreadId() => ThreadId = Thread.CurrentThread.ManagedThreadId;

        public override string ToString() => $"ThreadId: {ThreadId}";
    }
}