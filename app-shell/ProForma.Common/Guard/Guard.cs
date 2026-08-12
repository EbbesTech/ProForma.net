using ProForma.Shared.Guard;

namespace ProForma.Common.Guard;

public class Guard<T> : IGuard<T>
    where T : Exception, new()
{
    /// <summary>
    /// A little helper to catch exceptions triggert by the given action, 
    /// and use these exception as innerException.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    private static IGuardResult<T> TryCatch<TIn>(Func<bool> action)
    {
        try
        {
            return new GuardResult<T>(action());
        }
        catch (Exception ex)
        {
            return new GuardResult<T>(true, ex);
        }
    }

    public static IGuardResult<T> Assert<TIn>(TIn item, Func<TIn, bool> action)
    {
        return TryCatch<T>(() => !action(item));
    }

    public static IGuardResult<T> IsImplementingInterface<TIn, TCheck>()
    {
        return TryCatch<T>(() => typeof(TIn).GetInterface(typeof(TCheck).Name!) is null);
    }

    public static IGuardResult<T> IsAssignableToType<TIn, TCheck>()
    {
        return TryCatch<T>(() => !typeof(TIn).IsAssignableTo(typeof(TCheck)));
    }

    public static IGuardResult<T> IsEqualTo<TIn>(TIn itemA, TIn itemB)
    {
        return TryCatch<T>(() => {
            var compare = Comparer<TIn>.Default.Compare(itemA, itemB);
            return compare != 0;
        });
    }

    public static IGuardResult<T> IsGreaterThan<TIn>(TIn itemA, TIn itemB)
    {
        return TryCatch<T>(() => {
            var compare = Comparer<TIn>.Default.Compare(itemA, itemB);
            return compare <= 0;
        });
    }

    public static IGuardResult<T> IsGreaterThanOrEqualTo<TIn>(TIn itemA, TIn itemB)
    {
        return TryCatch<T>(() => {
            var compare = Comparer<TIn>.Default.Compare(itemA, itemB);
            return compare < 0;
        });
    }

    public static IGuardResult<T> IsLessThan<TIn>(TIn itemA, TIn itemB)
    {
        return TryCatch<T>(() => {
            var compare = Comparer<TIn>.Default.Compare(itemA, itemB);
            return compare >= 0;
        });
    }

    public static IGuardResult<T> IsLessThanOrEqualTo<TIn>(TIn itemA, TIn itemB)
    {
        return TryCatch<T>(() => {
            var compare = Comparer<TIn>.Default.Compare(itemA, itemB);
            return compare > 0;
        });
    }

    public static IGuardResult<T> IsNotNull<TIn>(TIn item)
    {
        return TryCatch<T>(() => item is null);
    }

    public static IGuardResult<T> IsNotNullOrEmpty(string item)
    {
        return TryCatch<T>(() => string.IsNullOrEmpty(item));
    }

    public static IGuardResult<T> IsNotNullOrWhiteSpace(string item)
    {
        return TryCatch<T>(() => string.IsNullOrWhiteSpace(item));
    }

    public static IGuardResult<T> IsNull<TIn>(TIn item)
    {
        return TryCatch<T>(() => item is not null);
    }

    public static IGuardResult<T> IsTrue(bool expression)
    {
        return TryCatch<T>(() => expression);
    }

    public static IGuardResult<T> IsFalse(bool expression)
    {
        return TryCatch<T>(() => !expression);
    }
}
