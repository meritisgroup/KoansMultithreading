using System.Threading.Tasks;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract
{
    public interface IAsynchroniousWrapper<T>
    {
        TaskCompletionSource<T> Handler { get; }
        Task Run();
    }
}