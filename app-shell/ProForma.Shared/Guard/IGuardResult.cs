namespace ProForma.Shared.Guard;

public interface IGuardResult<T>
    where T : Exception
{
    bool HasFailed { get; }
    Exception? InnerException { get; }
    void Throw(string? message = null);
}
