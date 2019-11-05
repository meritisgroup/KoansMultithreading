using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using KoansBase.Engine;
using KoansMultithreading.AboutThreadSynchronizationObject.NoChangeAllow;
using KoansMultithreading.AboutThreadSynchronizationObject.CanBeChanged;

namespace KoansMultithreading
{
    public class AboutThreadSynchronization : Koan
    {
        /// <summary>
        /// Some operator are a bit mysterious with Task. One of them allow to yield on a Task,
        /// that mean to use a continuation on the rest of the method. In other word, the same method
        /// can have different part in a different thread. Find it (use documentation!!!)
        /// </summary>
        /// <returns>Task only for synchronous purpose.</returns>
        [Step(1)]
        public void WhatMeanYieldOnTask()
        {
            var d = new Dictionary<int, int>();

            for (int i = 0; i < 10000; i++)
            {
                int id = Thread.CurrentThread.ManagedThreadId;
                d[id] = d.TryGetValue(id, out int count) ? count + 1 : 1;
                //missing something ?
            }
            Assert.True(d.Count > 1);
        }

        /// <summary>
        /// <para>
        /// Can you figure out how Async/Await works ? Can you identify how it is relied on Continuation ?
        /// Continuation is one of the powerful concept in computer science. We try here to present one way
        /// to exploit it.
        /// Here we aim at running all continuation into the same thread. It can be useful to know about this
        /// when we want to avoid context switching. Its some kind of fiber, or thread affinity depending on how
        /// you see that.
        /// Here some resources:
        /// https://en.wikipedia.org/wiki/Fiber_(computer_science)
        /// https://docs.microsoft.com/fr-fr/dotnet/api/system.diagnostics.processthread.processoraffinity?view=netframework-4.8
        /// https://stackoverflow.com/questions/2510593/how-can-i-set-processor-affinity-in-net
        /// </para>
        /// <para>
        /// This exercise is from the following post, please before reading it try to solve it !!!
        /// https://blogs.msdn.microsoft.com/pfxteam/2012/01/20/await-synchronizationcontext-and-console-apps/
        /// </para>
        /// </summary>
        [Step(2)]
        public void AboutSynchronizationContext()
        {
            var d = new Dictionary<int, int>();

            ISynchronizationContextFactory synchronizationContextFactory = FILL_ME_IN;

            var synchronizationContext = synchronizationContextFactory.Create();
            var threadPump = new TaskRunner(synchronizationContext,
                (_) => (synchronizationContext as IConsumerSideSynchronizationContext)?.Complete(),
                // Run continuations and propagate any exceptions
                (Task task) =>
                {
                    (synchronizationContext as IConsumerSideSynchronizationContext)?.RunOnCurrentThread();
                    task.GetAwaiter().GetResult();
                });
            threadPump.Run(async () => await AboutThreadSynchronizationHelper.PerformTaskAsync(d).ConfigureAwait(false));
            Assert.True(d.Count == 1);
        }

        #region Old [Synchronization] attribute (not available in DotNet core 3 ?
        /// <summary>
        /// ContextBoundObject is an abstraction allowing some kind of autolocking behaviour.
        /// https://docs.microsoft.com/fr-fr/dotnet/api/system.contextboundobject?view=netframework-4.8
        /// https://blogs.msdn.microsoft.com/tilovell/2011/02/06/contextboundobject-part-1-making-contexts/
        /// We try to implement a counter with autolock.
        /// </summary>

        //[Step(3)]
        //public void AboutContextBoundObject()
        //{
        //    var maxValue = 10;
        //    var expectedValue = (maxValue * (maxValue + 1)) / 2;
        //    Task[] tasks = new Task[maxValue];
        //    IAutolockCounter<int> counter = new AutolockCounter<int>(0, (a, b) => a + b);
        //    for (int idx = 0; idx < maxValue; idx++)
        //    {
        //        var capturedValue = idx;
        //        tasks[idx] = Task.Run(() => counter.Add(capturedValue));
        //    }
        //    Task.WaitAll(tasks);
        //    Assert.Equal(expectedValue, counter.Value);
        //}

        //public interface IAutolockCounter<T>
        //{
        //    T Value { get; }
        //    void Add(T value2BeAdded);
        //}

        //[Synchronization]
        //public class AutolockCounter<T> : ContextBoundObject, IAutolockCounter<T> where T : struct
        //{
        //    private readonly Func<T, T, T> _addOperation;
        //    private Nullable<T> _value;

        //    public AutolockCounter(T initialValue, Func<T, T, T> addOperation)
        //    {
        //        Value = initialValue;
        //        _addOperation = addOperation;
        //    }

        //    public T Value
        //    {
        //        get { return _value.Value; }
        //        private set => _value = new Nullable<T>(value);
        //    }

        //    public void Add(T value2BeAdded) => Value = _addOperation(value2BeAdded, Value);
        //} 
        #endregion



        /// <summary>
        /// Monitor is a class behind the Lock model in dotnet. But it can be used as a synchronization system.
        /// We have used it previously. We want to present here something which should be know before to use it ;)
        /// </summary>
        [Step(5)]
        public void AboutMonitorPulse()
        {
            var locker = new object();
            var workHasBeenDone = false;
            var timeOut = false;
            var synchronizedJobs = new Task[2];
            //another to perform a timeout
            var timeOutControl = new Task[2];
            void LongRunningJob()
            {
                Thread.Sleep(2000);
                lock (locker)
                {
                    Monitor.Pulse(locker);
                }
            }
            void ValidatorJob()
            {
                Thread.Sleep(3000);
                lock (locker)
                {
                    Monitor.Wait(locker);
                }
                workHasBeenDone = true;
            }
            void Wait()
            {
                Thread.Sleep(5000);
                timeOut = true;
            }

            synchronizedJobs[0] = Task.Run(LongRunningJob);
            synchronizedJobs[1] = Task.Run(ValidatorJob);
            timeOutControl[0] = Task.WhenAll(synchronizedJobs);
            timeOutControl[1] = Task.Run(Wait);
            Task.WhenAny(timeOutControl).Wait();

            // Assert all is OK
            Assert.False(timeOut);
            Assert.True(workHasBeenDone);
        }


        /// <summary>
        /// OK Monitor is a technique to handle Shared resource and synchronized around it.
        /// This post is interesting about Monitor vs WaitHandler (and the issue with signal
        /// lost with Monitor.Pulse like in the previous step):
        /// https://stackoverflow.com/questions/1355398/monitor-vs-waithandle-based-thread-sync
        /// Now let have a look at the waitHandler:
        /// https://docs.microsoft.com/fr-fr/dotnet/api/system.threading.waithandle?view=netcore-3.0
        /// The simplest one is AutoResetEvent.
        /// https://docs.microsoft.com/fr-fr/dotnet/api/system.threading.autoresetevent?view=netcore-3.0
        ///</summary>
        [Step(6)]
        public void BasicOfAutoresetEvent()
        {
            var are = new AutoResetEvent(true);
            bool firstJobDone = false;
            bool secondJobDone = false;

            void FirstJob()
            {
                Thread.Sleep(5000);
                firstJobDone = true;
                are.Set();
            }
            void ThenJob()
            {
                are.WaitOne();
                secondJobDone = true;
            }
            _ = Task.Run(FirstJob);
            Task.Run(ThenJob).Wait();

            // ----------------------------------------------------------------------------------
            // ----------------              IMPORTANT NOTICE HERE               ----------------
            // !!!!!!!!!!!!!!!!            WaitHandle must be close              !!!!!!!!!!!!!!!!
            // !!!!!!!!!!!!!!!!            because they use windows              !!!!!!!!!!!!!!!!
            // !!!!!!!!!!!!!!!!            handle for most of them               !!!!!!!!!!!!!!!!
            // !!!!!!!!!!!!!!!!            (except Slim ones)                    !!!!!!!!!!!!!!!!
            // ----------------------------------------------------------------------------------
            are.Close();
            // ----------------------------------------------------------------------------------

            Assert.True(firstJobDone && secondJobDone);
        }

        /// <summary>
        /// It is not design proof but the idea is to understand the underlying of
        /// synchronizing.
        /// Don't change anything in that code. You must implement the class produce
        /// by the factory for the AutoResetEvent. 
        /// </summary>
        [Step(7)]
        public void AboutSynchronizingJobWithAutoResetEvent()
        {
            RunCountWithSynchronization(SynchronisationTestCase.AutoReset);
        }

        /// <summary>
        /// Don't change anything in that code. You must implement the class produce
        /// by the factory for the ManualResetEvent. 
        /// </summary>
        [Step(8)]
        public void AboutSynchronizingJobWithManualResetEvent()
        {
            RunCountWithSynchronization(SynchronisationTestCase.ManualReset);
        }

        /// <summary>
        /// Don't change anything in that code. You must implement the class produce
        /// by the factory for the Thread Sleep case. 
        /// But what i the purpose of synchronizing with Thread Sleep ?
        /// What happen when you use <code>Thread.Sleep(0)</code> ?
        /// </summary>
        [Step(9)]
        public void AboutSynchronizingJobWitThreadSleep()
        {
            RunCountWithSynchronization(SynchronisationTestCase.ThreadSleep);
        }

        /// <summary>
        /// Don't change anything in that code. You must implement the class produce
        /// by the factory for the SpinWait case. Do you know SpinWait ?
        /// https://docs.microsoft.com/fr-fr/dotnet/api/system.threading.thread.spinwait?view=netcore-3.0
        /// But what is the difference with Thread Sleep ? ;)
        /// </summary>
        [Step(9)]
        public void AboutSynchronizingJobWitSpinWait()
        {
            RunCountWithSynchronization(SynchronisationTestCase.SpinWait);
        }


        /// <summary>
        /// Helper function to manipulate different case of synchronization.
        /// </summary>
        /// <param name="testCase">The synchronization case to be tested. 
        /// See <see cref="SynchronisationTestCase"/> for an enumeration of those cases.</param>
        private static void RunCountWithSynchronization(SynchronisationTestCase testCase)
        {
            const int totalIteration = 10;
            const int expectedCount = (totalIteration * (totalIteration + 1)) / 2;
            var factory = new SynchronizedRunnerFactory<int>();
            var count = 0;
            var runner = factory.Create(testCase, i => count += i);
            try
            {
                for (int idx = 1; idx <= totalIteration; idx++)
                {
                    runner.SetData(idx);
                }
            }
            finally
            {
                ((IDisposable)runner).Dispose();
            }
            Assert.Equal(expectedCount, count);
        }
    }
}
