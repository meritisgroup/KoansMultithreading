using System;
using System.Threading.Tasks;
using KoansMultithreading.AboutTaskObject.NoChangeAllow;
using KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract;

namespace KoansMultithreading.AboutTaskObject.CanBeChanged
{
    public class ArrayTaskFactory
    {
        public Task<ThreadLog<TOut>>[] BuildTasksTab<TOut>(int nbTasks, Func<TOut> worker)
        {
            var tasks = new Task<ThreadLog<TOut>>[nbTasks];
            ThreadLog<TOut> DoWork(int TaskId)
            {
                var log = new ThreadLog<TOut>();
                log.SetThreadId();
                log.TaskId = TaskId;
                log.Result = worker();
                return log;
            }
            for (int taskId = 0; taskId < nbTasks; taskId++)
            {
                tasks[taskId] = Task<ThreadLog<TOut>>.Factory.StartNew(() => DoWork(taskId));
            }
            return tasks;
        }

        public Task<ThreadLog<TOut>>[] BuildTasksTab<TOut>(int nbTasks, IWorkerFactory<TOut> factory)
        {
            var tasks = new Task<ThreadLog<TOut>>[nbTasks];
            ThreadLog<TOut> DoWork(int TaskId)
            {
                var log = new ThreadLog<TOut>();
                log.SetThreadId();
                log.TaskId = TaskId;
                // double parenthesis due to that the factory ouput a functor that can be called.
                log.Result = factory.CreateWorker()();
                return log;
            }
            for (int taskId = 0; taskId < nbTasks; taskId++)
            {
                // you realy should pay attention here ;)
                tasks[taskId] = Task<ThreadLog<TOut>>.Factory.StartNew(() => DoWork(taskId));
            }
            return tasks;
        }
    }
}