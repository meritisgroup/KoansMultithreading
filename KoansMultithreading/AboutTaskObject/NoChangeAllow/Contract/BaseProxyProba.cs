using System;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract
{
    public abstract class BaseProxyProba : IProxyProba
    {
        private readonly Func<double> _computeProba;
        private double _proba;

        public double Proba
        {
            get
            {
                Compute();
                return _proba;
            }
        }

        protected BaseProxyProba(Func<double> computeProba) => _computeProba = computeProba;

        protected void Compute() => _proba = _computeProba();
    }
}