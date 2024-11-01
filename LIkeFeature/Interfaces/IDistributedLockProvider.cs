namespace LIkeFeature.Interfaces
{
    public interface IDistributedLockProvider
    {
        Task<IDistributedLockHandle> AcquireLockAsync(string key, TimeSpan timeOut);
    }

    public interface IDistributedLockHandle : IDisposable
    {
        bool IsAcquired { get; }
    }
}
