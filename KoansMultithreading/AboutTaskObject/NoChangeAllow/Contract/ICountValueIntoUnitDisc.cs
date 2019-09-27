using KoansMultithreading.GlobalTools.NoChangeAllow;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract
{
    public interface ICountValueIntoUnitDisc
    {
        int CountIn { get; }
        int TotalCount { get; }

        double ProbaInUnitCircle();

        void HandleNewPoint(Point point);
    }
}