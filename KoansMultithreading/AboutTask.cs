using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using KoansBase.Engine;
using KoansMultithreading.GlobalTools.Contract;
using KoansMultithreading.GlobalTools.NoChangeAllow;
using KoansMultithreading.AboutThreadObject.NoChangeAllow;
using KoansMultithreading.AboutTaskObject.CanBeChanged;
using KoansMultithreading.AboutTaskObject.NoChangeAllow;
using KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract;
using Xunit;

namespace KoansMultithreading
{
    // The final exercice of Thread Class look up aim at introducing Task. You now have an idea of what to be do to implement Task.
    // We now target an exploration of Task and how to use it. Many option are available and gain a knowledge of them provide a better
    // overview of what can be done with it. Then we look at the different way for us to synchronize Task but keep being in the "Task universe".
    // As using thread imply a litle bit of synchronization, we only use here Monitor based method. So :
    // - Monitor.Enter
    // - Monitor.Exit
    // - lock
    // - Monitor.Pulse
    // - Monitor.Wait
    // In AboutTaskObject namespace, you can find class that can be change to solve the problem (CanBeCHanged nameSpace).
    // You also find in NoChangeAllow namespace contract that are clue to solve the problem (given an API you have to code concrete class that are
    // ask in the problem. You find also class that that used into the problem.
    // Let's dive into Task, Async/Await and so on... usages !!!!
    // What you should read :
    // https://devblogs.microsoft.com/pfxteam/task-run-vs-task-factory-startnew/
    // https://blog.stephencleary.com/2013/08/startnew-is-dangerous.html
    //
    // - http://blog.stephencleary.com/2013/11/there-is-no-thread.html

    public class AboutTask : Koan
    {
        private static readonly object _locker = new object();
        private readonly int sleepTime = 5000;

        private const double epsilon = 0.00001;

        // IS task bring the same issue as with thread ?
        [Step(1)]
        public void PassArgumentIntoThread()
        {
            // Parameters
            int nbThread = 10;
            int expectedSum = (nbThread * (nbThread + 1)) / 2;
            // still concurent sum of integer from 1 to n but using object as parameter
            MutableInt sum = FILL_ME_IN;
            var summer = AboutTaskHelper.GetSumAction(sum);
            // Something weird here ?
            for (int i = 1; i <= nbThread; i++)
            {
                // the behaviour is not deterministic without a few change ...
                // hint: have a close look at the closure
                new Task(() => summer(i)).Start();
            }
            // the worst way to synchronize task ;)
            Task.Delay(5000);
            // Assert
            Assert.Equal(expectedSum, sum.Value);
        }

        // About
        // FAQ : https://devblogs.microsoft.com/pfxteam/asyncawait-faq/
        //
        //      About TaskCompletionSource usage
        // doc here : https://docs.microsoft.com/fr-fr/dotnet/api/system.threading.tasks.taskcompletionsource-1?view=netframework-4.8
        // nice usage explanation here : https://www.pluralsight.com/guides/task-taskcompletion-source-csharp
        // usefull example usage here : https://stackoverflow.com/questions/15316613/when-should-taskcompletionsourcet-be-used
        //

        [Step(2)]
        public async void WritingResultAsynchroniouslyOnSynchroneApi()
        {
            int expectedValue = 42;
            var api = new SynchroneAPI();
            IAsynchroniousWrapper<int> runner = FILL_ME_IN;
            await runner.Run();
            int result = runner.Handler.Task.Result;
            Assert.Equal(expectedValue, result);
        }

        [Step(3)]
        public async void HandlingExceptionAsynchroniouslyOnSynchroneApi()
        {
            var api = new SynchroneAPI();
            IAsynchroniousWrapper<int> runner = FILL_ME_IN;
            await runner.Run();
            Assert.Equal(TaskStatus.Faulted, runner.Handler.Task.Status);
            Assert.IsType<AggregateException>(runner.Handler.Task.Exception);
            Assert.IsType<TimeoutException>(runner.Handler.Task.Exception.InnerException);
        }

        // Usefull when you get old style event base asynchronious pattern. The main issue with this pattern is that a
        // callback hell that can appear.
        // https://docs.microsoft.com/fr-fr/dotnet/standard/asynchronous-programming-patterns/event-based-asynchronous-pattern-overview
        [Step(4)]
        public async void WritingResultAsynchroniouslyOnEventBaseApi()
        {
            int expectedValue = 42;
            var api = new EventBasedAPI();
            IAsynchroniousWrapper<int> runner = FILL_ME_IN;
            await runner.Run();
            int result = runner.Handler.Task.Result;
            Assert.Equal(expectedValue, result);
        }

        // Based on this article, even if it is not consider as a bad practice, async void event handlers has some unpleasant drawback.
        // http://gigi.nullneuron.net/gigilabs/the-dangers-of-async-void-event-handlers/
        [Step(5)]
        public void OnAsyncVoidEventHandlers()
        {
            var _locker = new object();
            // this kind of declaration does not happend in real life but remind it is just a contextual mock.
            IMessageAPI msgApi = new MessageQueue();
            IMessageSender sender = msgApi as IMessageSender;
            // our message worker
            var msgWorker = new MessageConsummer();
            // register our action to be done on each recieved message
            msgApi.Received += msgWorker.Message_Received;
            // -----------------------------------------
            void MessageProducer()
            {
                for (int i = 0; i < 10; i++)
                {
                    var msg = new Message("Test context", $"{i}");
                    sender.Push(msg);
                }
                lock (_locker)
                {
                    Monitor.Pulse(_locker);
                }
            }
            (new Task(MessageProducer)).Start();
            lock (_locker)
            {
                Monitor.Wait(_locker);
            }
            var count = 0;
            msgWorker.Foreach(MessageConsummerCase.Received, (msg) => Assert.Equal($"Received: {count++}", msg));
            count = 0;
            msgWorker.Foreach(MessageConsummerCase.Processed, (msg) => Assert.Equal($"Processed: {count++}", msg));
        }

        // Based on this article, even if it is not consider as a bad practice, async void event handlers has some unpleasant drawback.
        // http://gigi.nullneuron.net/gigilabs/the-dangers-of-async-void-event-handlers/"
        // Here we show that you have no mean to handle error with async void event handlers.
        [Step(6)]
        public void OnAsyncVoidEventHandlersAndExceptionHandling()
        {
            var _locker = new object();
            // this kind of declaration does not happend in real life but remind it is just a contextual mock.
            IMessageAPI msgApi = new MessageQueue();
            IMessageSender sender = msgApi as IMessageSender;
            // our message worker
            var msgWorker = new MessageConsummer();
            // register our action to be done on each recieved message
            // this kind of pattern can be found with RabbitMQ client library for example,
            // it is a real life pattern and we should pay attention to not fall into a trap.
            msgApi.Received += msgWorker.Message_Received_But_Exception_On_processing;
            msgApi.OnError += msgWorker.ErrorHandler;
            // -----------------------------------------
            var msg = new Message("Test context", $"0");
            void MessageProducer()
            {
                sender.Push(msg);
                lock (_locker)
                {
                    Monitor.Pulse(_locker);
                }
            }
            (new Task(MessageProducer)).Start();
            lock (_locker)
            {
                Monitor.Wait(_locker);
            }
            var count = 0;
            msgWorker.Foreach(MessageConsummerCase.Error, (errorMsg) => { Assert.Equal($"Error on: {msg.Body} with error type: {typeof(TimeoutException).Name}", errorMsg); count++; });
            Assert.Equal(1, count);
            msgWorker.Foreach((ex) => Assert.Equal("Can not process message in time.", ex.Message));
        }

        //      About fireAndForget AsyncTask
        // You sometime want to run task but keep the order safe ... A classic mistake is do parallelism instead of asynchronicity.
        // What is the difference between theexactly, can you answer this question ?
        // It is two distinct model of concurency but can you tell more ?
        // Here the code is run un parralel is it a good model ? What do you gain by running it asynchroniously ?
        // Some additionnal interresting remark on async void method :
        // https://www.pluralsight.com/guides/returning-void-from-c-async-method
        // you can also remind that there is no mean to await async void method.
        [Step(7)]
        public void FireAndForgetTaskAndAsync()
        {
            var _locker = new object();
            var length = 10;
            var tasks = new int[length];
            var count = 1;
            int GetCountValue()
            {
                var toBeReturned = 0;
                lock (_locker)
                {
                    toBeReturned = count++;
                }
                return toBeReturned;
            }
            for (int i = 0; i < length; i++)
            {
                var it = i;
                (new Task(() => tasks[it] = GetCountValue())).Start();
            }
            Thread.Sleep(2000);
            for (int i = 0; i < length; i++)
            {
                Assert.Equal(i, tasks[i]);
            }
        }

        // further reading (and really interesting):
        // https://msdn.microsoft.com/en-us/magazine/jj991977.aspx

        // About Parallelism

        // Simple way to synchronize to wait for execution of multiple task.
        // do not use 'await' keyword
        [Step(8)]
        public void OnASimpleWayToSynchornizeThread_1()
        {
            var nbTask = 10;
            var tasks = (new ArrayTaskFactory()).BuildTasksTab(nbTask, new RandomWalker().CrossZero);
            // FILL_ME_IN;
            var maybeResults = tasks.Select(task => task.Status == TaskStatus.Running ? null : task.Result).ToList();
            foreach (ThreadLog<int> result in maybeResults)
            {
                Assert.NotNull(result);
            }
        }

        // Yet another simple way to synchronize to wait for execution of multiple task but asynchroniously.
        // Same as previous test but here you must use 'await' keyword.
        // -----------------------                          REMARK                           -----------------------
        // Method from this step and the previous one are quite similar in appearance (exept that you can await here)
        // but in the step 11 you can see another difference by looking into commentary. Try it !!!
        // ---------------------------------------------------------------------------------------------------------
        [Step(9)]
        public void OnASimpleWayToSynchornizeThread_2()
        {
            var nbTask = 10;
            var tasks = (new ArrayTaskFactory()).BuildTasksTab(nbTask, new RandomWalker().Walk);
            // FILL_ME_IN;
            var maybeResults = tasks.Select(task => task.Status == TaskStatus.Running ? null : task.Result).ToList();
            foreach (ThreadLog<int> result in maybeResults)
            {
                Assert.NotNull(result);
            }
        }

        // Just a little reminder to be sure that the basic deadlock can happened with tTPL also.
        // No systeme is available into the TPL to prevent from deadlock.
        [Step(10)]
        public void OnSharedRessource()
        {
            var obj1 = new object();
            var obj2 = new object();

            Task[] tasks = new Task[2];

            var haveBeenCompletedFirstLongTask = false;
            var haveBeenCompletedSecondLongTask = false;

            var jobFactory = new SimpleJob();

            void RunLongTask(object locker1, object locker2, ref bool haveBeenCompleted)
            {
                lock (locker1)
                {
                    jobFactory.DoCorseGrainedJob(1000);
                    lock (locker2)
                    {
                        jobFactory.DoCorseGrainedJob(1000);
                        haveBeenCompleted = true;
                    }
                }
            }

            tasks[0] = new Task(() => RunLongTask(obj1, obj2, ref haveBeenCompletedFirstLongTask));
            tasks[1] = new Task(() => RunLongTask(obj2, obj1, ref haveBeenCompletedSecondLongTask));

            tasks[0].Start();
            tasks[1].Start();

            // FILL_ME_IN

            Assert.True(haveBeenCompletedFirstLongTask);
            Assert.True(haveBeenCompletedSecondLongTask);
        }

        // First way to handle exception into Task
        [Step(11)]
        public void AboutErrorHandlingIntoTask()
        {
            var task = new Task(() =>
            {
                Thread.Sleep(1000);
                throw new TestException("test");
            });
            task.RunSynchronously();
            var ex = FILL_ME_IN;
            Assert.IsType<TestException>(ex);
            Assert.Single(task.Exception.InnerExceptions);
        }

        // Handling exception into 'await' Task
        [Step(12)]
        public async void AboutErrorHandlingIntoAwaitedTask()
        {
            Task Worker()
            {
                var task = new Task(() =>
                {
                    Thread.Sleep(1000);
                    throw new TestException("test");
                });
                task.RunSynchronously();
                return task;
            }
            await Worker();
            var ex = FILL_ME_IN;
            Assert.IsType<TestException>(ex);
        }

        // Easy Continuation with task
        [Step(13)]
        public void EasyContinuationWithTask()
        {
            var nbTask = 10;
            Task<ThreadLog<int>>[] tasks = (new ArrayTaskFactory()).BuildTasksTab(nbTask, FILL_ME_IN);
            // Use continuation to compute mean over result on tasks
            double result = FILL_ME_IN;
            Assert.NotEqual(0.0, result);
        }

        // Handle Cancellation
        [Step(14)]
        public async void OnCancellationWithTask()
        {
            var nbTask = 10;
            bool cancelled = false;
            Task<ThreadLog<int>>[] tasks = (new ArrayTaskFactory()).BuildTasksTab(nbTask, FILL_ME_IN);
            // Try use Task.WaitAll(tasks) or Task.WhenAll(tasks) here ... What can you deduce ?
            // https://docs.microsoft.com/fr-fr/dotnet/standard/parallel-programming/task-based-asynchronous-programming#handling-exceptions-in-tasks
            await Task.WhenAll(tasks);
            Assert.True(cancelled);
        }

        // Handle TimeOut
        [Step(15)]
        public void OnTimeoutWithTask()
        {
            var timeOut = 500;
            var nbIteration = 10;
            var timeSlicing = 100;
            var timeElapsed = 0;
            void Worker()
            {
                for (int idx = 0; idx < nbIteration; idx++)
                {
                    Thread.Sleep(timeSlicing);
                    timeElapsed += timeSlicing;
                }
            }
            var work = Task.Run(Worker);
            work.Wait();
            Assert.True(timeElapsed <= timeOut);
        }

        // Child Task and execcution order
        // Pay attention to take a deeper look at this article:
        // https://devblogs.microsoft.com/pfxteam/task-run-vs-task-factory-startnew/
        [Step(16)]
        public async void OnChildTaskCompletionOrder()
        {
            var nbTask = 10;
            var isParentLast = new MutableBool();
            await Task.Run(() =>
            {
                Task<ThreadLog<bool>>[] tasks = FILL_ME_IN;
            }).ContinueWith((t) => isParentLast.SetTrueAtomic());
            Assert.True(isParentLast.Value);
        }

        // There is many other useful feature into to. I strongly encourage you to look
        // by yourself for more detail. For now we want to introduce a last pain point in
        // multitasking: collection management. Microsoft have long ago introduced threadsafe
        // collection. Let's have a look at some of them.

        // Dictionnary can have some undesired (and quite astonishing) effects ...
        // And all other thread unsafe collections have their own way to working unexpectingly ;)
        [Step(17)]
        public void OnUnprotectedUsageOfDictionnary()
        {
            var dictionary = new Dictionary<int, int>();
            try
            {
                Enumerable.Range(0, 1000000)
                    .ToList()
                    .AsParallel()
                    .ForAll(n =>
                    {
                        if (!dictionary.ContainsKey(n))
                        {
                            dictionary[n] = n; //write
                        }
                        var readValue = dictionary[n]; //read
                        Assert.Equal(n, readValue);
                    });
            }
            catch (AggregateException e)
            {
                e.Flatten()
                    .InnerExceptions.ToList()
                    .ForEach(i => Console.WriteLine(i.Message));
            }
        }

        // Some other collections have been added for multitasking usage :
        // - BlockingCollection<T> -> https://docs.microsoft.com/fr-fr/dotnet/api/system.collections.concurrent.blockingcollection-1?view=netframework-4.8
        // - ConcurrentBag<T>      -> https://docs.microsoft.com/fr-fr/dotnet/api/system.collections.concurrent.concurrentbag-1?view=netframework-4.8
        // Remark : there is no implementation in Generic.Collection of Bag. The concurent version is the only one that can be found.
        [Step(18)]
        public void EasyProducterConsumerWithBlockingCollection()
        {
            IProducer<Point> producer = FILL_ME_IN;
            IConsumerFactory<Point> computeProbaFactory = FILL_ME_IN;
            // no change required from here
            var randomPointGenerator = new RandomPointGenerator(producer);
            var (piEvaluator, consumer) = (new PiEvaluatorFactory(computeProbaFactory)).CreateInstance();
            Task consumerTask;
            Task producerTask;
            consumerTask = consumer.StartConsume();
            producerTask = randomPointGenerator.GeneratePoints(10_000_000).ContinueWith(t => consumer.StopConsume());
            Task.WaitAll(consumerTask, producerTask);
            Assert.True(Math.Abs(piEvaluator.Pi - 3.1415) < epsilon);
        }

        // Here prove the last try by using your code in multiple producer scenario.
        [Step(19)]
        public void MultipleProducterConsumerWithBlockingCollection()
        {
            IProducer<Point> producer = FILL_ME_IN;
            IConsumerFactory<Point> computeProbaFactory = FILL_ME_IN;
            // no change required from here
            var randomPointGenerator = new RandomPointGenerator(producer);
            var (piEvaluator, consumer) = (new PiEvaluatorFactory(computeProbaFactory)).CreateInstance();
            Task consumerTask;
            Task[] producerTask;
            consumerTask = consumer.StartConsume();
            producerTask = randomPointGenerator.GeneratePoints(10, 10_000_000);
            Task.WhenAll(producerTask).ContinueWith(t => consumer.StopConsume());
            Task.WaitAll(consumerTask);
            Assert.True(Math.Abs(piEvaluator.Pi - 3.1415) < epsilon);
        }

        // Same exercice using Threading channel.
        // Documenttion of static helper class (factory method)
        // https://docs.microsoft.com/fr-fr/dotnet/api/system.threading.channels.channel?view=dotnet-plat-ext-2.1
        // Documenttion of class
        // https://docs.microsoft.com/fr-fr/dotnet/api/system.threading.channels.channel-1?view=dotnet-plat-ext-2.1
        // Interesting article but try to read it after your first try :
        // https://www.stevejgordon.co.uk/an-introduction-to-system-threading-channels
        [Step(20)]
        public void EasyProducterConsumerWithThreadingChannel()
        {
            IProducer<Point> producer = FILL_ME_IN;
            IConsumerFactory<Point> computeProbaFactory = FILL_ME_IN;
            // no change required from here
            var randomPointGenerator = new RandomPointGenerator(producer);
            var (piEvaluator, consumer) = (new PiEvaluatorFactory(computeProbaFactory)).CreateInstance();
            Task consumerTask;
            Task producerTask;
            consumerTask = consumer.StartConsume();
            producerTask = randomPointGenerator.GeneratePoints(10_000_000).ContinueWith(t => consumer.StopConsume());
            Task.WaitAll(consumerTask, producerTask);
            Assert.True(Math.Abs(piEvaluator.Pi - 3.1415) < epsilon);
        }
    }
}