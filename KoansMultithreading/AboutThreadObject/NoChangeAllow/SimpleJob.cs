using System.Threading;

namespace KoansMultithreading.AboutThreadObject.NoChangeAllow
{
    public class SimpleJob
    {
        public void DoCorseGrainedJob(int timeToWork = 1000) => Thread.Sleep(timeToWork);
    }
}