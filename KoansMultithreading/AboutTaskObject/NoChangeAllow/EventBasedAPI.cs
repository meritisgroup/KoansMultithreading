using System;
using System.Threading;
using System.Threading.Tasks;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow
{
    public class EventBasedAPI
    {
        public event Action<int> OnAnswerFound;

        public void AnswerLookUp()
        {
            void Runner()
            {
                Thread.Sleep(1000);
                OnAnswerFound(42);
            }
            (new Task(Runner)).Start();
        }
    }
}