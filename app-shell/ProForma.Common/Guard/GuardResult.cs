using ProForma.Shared.Guard;

using System.Reflection;

namespace ProForma.Common.Guard;

public class GuardResult<T>(bool hasFailed, Exception? innerException = null) : IGuardResult<T>
    where T : Exception, new() 
{
    public bool HasFailed => hasFailed;

    public Exception? InnerException => innerException;

    public void Throw(string? message = null)
    {
        if (hasFailed)
        {
            var constructor = typeof(T).GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                null,
                [typeof(string), typeof(Exception)],
                null);

            if (constructor == null)
            {
                throw new InvalidOperationException(
                    $"Type {typeof(T).Name} does not have a constructor that accepts (string message, Exception innerException).");
            }

            throw (T)constructor.Invoke(new object[] { message!, innerException! });
        }
    }
}