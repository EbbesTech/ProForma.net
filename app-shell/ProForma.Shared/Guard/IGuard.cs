namespace ProForma.Shared.Guard;

public interface IGuard<T>
    where T : Exception
{
    static abstract IGuardResult<T> IsNotNull<TIn>(TIn item);
    static abstract IGuardResult<T> IsNull<TIn>(TIn item);
    static abstract IGuardResult<T> IsAssignableToType<TIn, TCheck>();
    static abstract IGuardResult<T> IsImplementingInterface<TIn, TCheck>();
    static abstract IGuardResult<T> Assert<TIn>(TIn item, Func<TIn, bool> action);
    static abstract IGuardResult<T> IsEqualTo<TIn>(TIn itemA, TIn itemB);
    static abstract IGuardResult<T> IsLessThan<TIn>(TIn itemA, TIn itemB);
    static abstract IGuardResult<T> IsLessThanOrEqualTo<TIn>(TIn itemA, TIn itemB);
    static abstract IGuardResult<T> IsGreaterThan<TIn>(TIn itemA, TIn itemB);
    static abstract IGuardResult<T> IsGreaterThanOrEqualTo<TIn>(TIn itemA, TIn itemB);
    static abstract IGuardResult<T> IsNotNullOrEmpty(string item);
    static abstract IGuardResult<T> IsNotNullOrWhiteSpace(string item);
    static abstract IGuardResult<T> IsTrue(bool expressionResult);
    static abstract IGuardResult<T> IsFalse(bool expressionResult);
}
