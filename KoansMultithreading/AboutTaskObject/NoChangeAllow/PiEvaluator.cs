using KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow
{
    /// <summary>
    /// An evaluator of Pi using monte carlo simulation. The proba used here is a proxy on the true probality of a random
    /// point drawn from the unit square (two dimensions) to be in the unit circle. This probability ca n be compute using
    /// geometry by the ratio of the area of the unit circle (equal to Pi exactly) and the area of the unit square (4).
    /// Consequently the approximation of Pi is equal to 4 * Proba.
    /// </summary>
    public class PiEvaluator : BaseProxyProba
    {
        /// <summary>
        /// approximate value of Pi using the monte carlo simulation technic.
        /// </summary>
        public double Pi => 4 * Proba;

        public PiEvaluator(ICountValueIntoUnitDisc counter) : base(counter.ProbaInUnitCircle)
        {
        }
    }
}