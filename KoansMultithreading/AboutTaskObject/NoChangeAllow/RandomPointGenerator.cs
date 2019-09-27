using System;
using System.Runtime.CompilerServices;
using MathNet.Numerics.Random;
using KoansMultithreading.GlobalTools.Contract;
using KoansMultithreading.GlobalTools.NoChangeAllow;
using System.Threading.Tasks;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow
{
    public sealed class RandomPointGenerator
    {
        private readonly IProducer<Point> _producer;
        private readonly Random _generator;

        public RandomPointGenerator(IProducer<Point> producer)
        {
            _producer = producer;
            _generator = new Xorshift(DateTime.UtcNow.Millisecond, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref Point GetNewPoint(ref Point point)
        {
            point = new Point(GetCoupleOfDouble());
            return ref point;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetDouble() => 2 * _generator.NextDouble() - 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double[] GetCoupleOfDouble() => new double[] { GetDouble(), GetDouble() };

        public Task GeneratePoints(int nbPointToBeGenerated) => Task.Run(() => Produce(nbPointToBeGenerated));

        public Task[] GeneratePoints(int nbTask, int nbPointToBeGenerated)
        {
            var tasks = new Task[nbTask];
            for (int idx = 0; idx < nbTask; idx++)
            {
                tasks[idx] = GeneratePoints(nbPointToBeGenerated);
            }
            return tasks;
        }

        private void Produce(int nbPointToBeGenerated)
        {
            Point point = new Point(0, 0);
            for (int idx = 0; idx < nbPointToBeGenerated; idx++)
            {
                ref Point refPoint = ref GetNewPoint(ref point);
                _producer.Post(refPoint);
            }
        }
    }
}