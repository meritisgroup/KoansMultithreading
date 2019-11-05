using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KoansMultithreading.AboutThreadSynchronizationObject.NoChangeAllow
{
    public static class AboutThreadSynchronizationHelper
    {
        public static async Task PerformTaskAsync(Dictionary<int, int> log)
        {
            for (int i = 0; i < 10000; i++)
            {
                int id = Thread.CurrentThread.ManagedThreadId;
                log[id] = log.TryGetValue(id, out int count) ? count + 1 : 1;
                await Task.Yield();
            }
        }
    }
}