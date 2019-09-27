using System;

namespace KoansMultithreading.AboutTaskObject.CanBeChanged
{
    public class RandomWalker
    {
        private Random Generator;

        public RandomWalker()
        {
            Generator = new Random(DateTime.UtcNow.Millisecond);
        }

        public int Walk()
        {
            int position = 0;
            for (int step = 0; step < 10000; step++)
            {
                position += Generator.Next(-100, 100);
            }
            return position;
        }

        public int CrossZero()
        {
            int position = Generator.Next(-100, 100);
            int count = 0;
            for (int step = 0; step < 9999; step++)
            {
                var oldPosition = position;
                position += Generator.Next(-100, 100);
                if (oldPosition * position < 0) count++;
            }
            return count;
        }
    }
}