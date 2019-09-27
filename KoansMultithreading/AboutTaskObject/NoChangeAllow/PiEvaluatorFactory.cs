using KoansMultithreading.GlobalTools.Contract;
using KoansMultithreading.GlobalTools.NoChangeAllow;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow
{
    /// <summary>
    /// To allow simple using of custom consumer into PiEvaluator.
    /// </summary>
    public class PiEvaluatorFactory
    {
        private readonly IConsumerFactory<Point> _factory;

        public PiEvaluatorFactory(IConsumerFactory<Point> factory) => _factory = factory;

        public (PiEvaluator, IConsumer) CreateInstance()
        {
            var counter = new CountValueIntoUnitDisc(_factory.CreateInstance());
            IConsumer consumer = counter;
            return (new PiEvaluator(counter), consumer);
        }
    }
}