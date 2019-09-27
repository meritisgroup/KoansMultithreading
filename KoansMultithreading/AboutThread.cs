using System;
using System.Threading;
using Xunit;
using KoansBase.Engine;
using KoansMultithreading.AboutThreadObject.CanBeChanged;
using KoansMultithreading.AboutThreadObject.NoChangeAllow;
using KoansMultithreading.AboutThreadObject.NoChangeAllow.Contract;

namespace KoansMultithreading
{
    // Fast lookup of Thread Class. This class has many way to be used. We only pretend to offer one among available possiblities.
    // As using thread imply a litle bit of synchronization, we only use here Monitor based method. So :
    // - Monitor.Enter
    // - Monitor.Exit
    // - lock
    // - Monitor.Pulse
    // - Monitor.Wait
    // In AboutThreadObject namespace, you can find class that can be change to solve the problem (CanBeCHanged nameSpace).
    // You also find in NoChangeAllow namespace contract that are clue to solve the problem (given an API you have to code concrete class that are
    // ask in the problem. You find also class that that used into the problem.
    // Let's dive into Thread class usage !!!!
    public class AboutThread : Koan
    {
        private static readonly object _object = new object();
        [Step(1)]
        public void CreateThread()
        {
            var jobFactory = new SimpleJob();
            var haveBeenExecuted = false;
            // Execute the job in a separate thread and be sure it has runned successfully
            var thread = new Thread(() =>
            {
                jobFactory.DoCorseGrainedJob(5000);
                haveBeenExecuted = true;
            });
            thread.Start();
            thread.Join();
            // Missing something ?
            // Assert
            Assert.True(haveBeenExecuted);
        }

        [Step(2)]
        public void LambdaIntoThread()
        {
            // Parameters
            int nbThread = 10;
            int expectedSum = (nbThread * (nbThread + 1)) / 2;
            // concurent sum of integer from 1 to n
            int sum = 0;
            // Something weird here ?
            for (int i = 1; i <= nbThread; i++)
            {
                // the behaviour is not deterministic without a few change ...
                // hint: have a close look at the closure
                new Thread(() =>
                {
                    lock (_object)
                    {
                        sum += i;
                    }
                }
                ).Start();
            }
            // Assert
            Assert.Equal(expectedSum, sum);
        }

        [Step(3)]
        public void PassArgumentIntoThread()
        {
            // Parameters
            int nbThread = 10;
            int expectedSum = (nbThread * (nbThread + 1)) / 2;
            // still concurent sum of integer from 1 to n but using object as parameter
            IntArgument sum = FILL_ME_IN;
            // Something weird here ?
            for (int i = 1; i <= nbThread; i++)
            {
                // the behaviour is not deterministic without a few change ...
                // hint: have a close look at the closure
                new Thread(IntArgument.GetSumParameterizedThreadStart(sum)).Start();
            }
            // Assert
            Assert.Equal(expectedSum, sum.Cast());
        }

        [Step(4)]
        // hint: no lock deadlock, but why ?
        public void OnSharedRessource()
        {
            var obj1 = new object();
            var obj2 = new object();

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

            var thread1 = new Thread(() => RunLongTask(obj1, obj2, ref haveBeenCompletedFirstLongTask));
            var thread2 = new Thread(() => RunLongTask(obj2, obj1, ref haveBeenCompletedSecondLongTask));

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.True(haveBeenCompletedFirstLongTask);
            Assert.True(haveBeenCompletedSecondLongTask);
        }

        [Step(5)]
        // Do a deadlock appear only when there is a lock
        // in the code ?
        // here the no lock deadlock, but why ?
        public void OnStaticConstructor()
        {
            Assert.Equal(3.14f, PiHelper.PI);
        }

        [Step(6)]
        // How to properly handle exception from Thread ?
        // Extra: factorize your idea into an object ;)
        // We may use it again soon !!!
        public void OnExceptionHandling()
        {
            Action MaybeThrow = () => { return; };

            void GreatHardJob() => throw new AboutThreadKoansException();

            try
            {
                var thread = new Thread(GreatHardJob);
                thread.Start();
                thread.Join();
            }
            catch (Exception e)
            {
                MaybeThrow = () => throw e;
            }

            Assert.Throws<AboutThreadKoansException>(MaybeThrow);
        }

        [Step(7)]
        // Hint: what do you know about cancellation into TPL ?
        //       have you code a class that can help you ?
        // Extra: factorize your idea into an object ;)
        // We may use it again soon !!!
        public void OnCancellingThread()
        {
            var jobFactory = new SimpleJob();
            var haveBeenExecuted = false;
            IResult result;
            // Execute the job in a separate thread and be sure it has runned successfully
            var thread = new Thread(() =>
            {
                jobFactory.DoCorseGrainedJob(5000);
                haveBeenExecuted = true;
            });
            thread.Start();
            thread.Interrupt();
            result = FILL_ME_IN;
            Assert.False(haveBeenExecuted);
            Assert.True(result.HasError);
            Assert.IsType<OperationCanceledException>(result.InternalError);
        }

        [Step(8)]
        public void OnThreadingContinuation()
        {
            // ARRANGE
            double Sum(double[] toBeSummed)
            {
                double total = 0.0;
                for (int it = 0; it < toBeSummed.Length; it++)
                {
                    total += toBeSummed[it];
                }
                Thread.Sleep(5000);
                return total;
            }

            var values = new double[] { 2.5023, 15.72473, 1.0308, 23.98, 6.8943 };
            double FinishMeanCompute(IResult<double> previousJobResult) => previousJobResult.Value / ((double)(values.Length));

            // ACT
            IJob<double> firstJob = FILL_ME_IN;
            firstJob.StartWith(() => Sum(values));
            IJob<double> secondJob = firstJob.Then(FinishMeanCompute);
            secondJob.Run().Wait();
            // ASSERT
            var res = secondJob.Result as IResult<double>;
            Assert.NotNull(res);
            Assert.True((res.Value - 10.026426) < 0.001);
        }

        [Step(9)]
        public void OnThreadingContinuationWithErrorHandling()
        {
            // ARRANGE
            double Sum(double[] toBeSummed)
            {
                double total = 0.0;
                for (int it = 0; it < toBeSummed.Length; it++)
                {
                    total += toBeSummed[it];
                }
                Thread.Sleep(5000);
                return total;
            }

            var values = new double[] { 2.5023, 15.72473, 1.0308, 23.98, 6.8943 };
            double FinishMeanCompute(IResult<double> previousJobResult) => ((int)previousJobResult.Value) / 0;

            // ACT
            IJob<double> firstJob = FILL_ME_IN;
            firstJob.StartWith(() => Sum(values));
            IJob<double> secondJob = firstJob.Then(FinishMeanCompute);
            secondJob.Run().Wait();
            // ASSERT
            var res = secondJob.Result as IResult<double>;
            Assert.NotNull(res);
            Assert.True(res.HasError);
            Console.WriteLine(res.InternalError);
        }
    }
}