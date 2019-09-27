using System.Threading;

namespace KoansMultithreading.AboutThreadObject.CanBeChanged
{
    public class PiHelper
    {
        // More details here, but wait before to read it, you had better
        // to think about it, and then think about it once again ;)
        // https://ericlippert.com/2013/01/31/the-no-lock-deadlock/

        public static float PI { get; set; }

        private static bool initialized = DoInitialize();

        private static bool DoInitialize()
        {
            if (!initialized)
            {
                var thread = new Thread(() => { PI = 3.14f; });
                thread.Start();
                thread.Join();
            }
            return true;
        }
    }
}