Hello I'm Marlene and I invite you to follow my journey developing ProForma.net.
But since this is my first post about ProForma, I will give you an overview of what I'm 
trying to achieve. 

## What ProForma.net is planned to be
The main Goal is to develop an application shell for schema based Applications. 
You will have mainly to different types of UI schemes, first for the Window Layout, there you will tell which elements are contained in the different application sections, like what Buttons or Menus you will have in the window title bar, or what sidebar tabs you will provide for the Ribbons, what the content area is filled with (spoiler I'm going to use flexlayout-react https://github.com/caplin/FlexLayout). As host application I will write a C# application using the WebView2 abstraction library Photino (https://www.tryphotino.io/). 

## What you can expect 
In this dev diary series I'll show what I was working on, I'll show you some code and will explain why did to choose the way I did it, or will share some thoughts about the project or the architecture. I also will show you how to write plugins for ProForma, because I plan to handle everything as a plugin so you can change the most aspects of the app. 

## The journey begins: overcome the guard
Ok, most of you will know it... parameter checking on top of a method... nearly endless 'if throw' constructs... they are ugly...   
```CS
if (!Directory.Exists(physicalPath)) throw new DirectoryNotFoundException($"Could not find the given path '{physicalPath}'.");

        if (_directories.ContainsKey(urlPrefix)) throw new Exception($"Key '{urlPrefix}' already exists.");

        if (_directories.ContainsValue(physicalPath)) throw new Exception($"Physical Path '{physicalPath}' already exists.");
```
I mean who wants to read that?
I don't. So I wanted guards, and I've could used some 3rd Party library, but instead I came up with my own solution for the Guards, since I don't always want to throw the exception on a failed assert, I split the guard in two parts, the guard who does the check and a guard result which contains the check result and a `Throw(string? message)`. 

First the `IGuardResult<T>` it's the result the guard will produce. I thought, I don't want to throw an exception just because the assertion failed, so I came to the following interface:
 
```CS
namespace ProForma.Shared.Guard;

public interface IGuardResult<T>
    where T : Exception
{
    bool HasFailed { get; }
    Exception? InnerException { get; }
    void Throw(string? message = null);
}
```
As you see, we have a property `HasFailed` which is used to check the result if you don't want to throw an exception. If you want to throw just call the Throw with an optional message. And as you see the class is generic the `T` has to be of an exception type. 

The implementation is found in `GuardResult<T>` nothing too special here, just perhaps how I create the exception object of `T` with the `Activator.CreateInstance`:
```CS
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
```
Then we have the `IGuard<T>` interface to describe what the guard should be capable of, I just assumed some methods that could be useful, I'm pretty sure there will come some more in the future. 
```CS
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
```
As you see the Result is always a `IGuardResult<T>` so can always decide if you want to throw an exception or handle the result manually. I also see that the `IGuard<T>` interface is a bit useless now, but when I decide to put the Guard into the DI I don't have to change much. 

Here you see my implementation of `IGuard<T>`, it is untested yet a can have some bugs especially in the Greater/Less methods. 
```CS
using ProForma.Shared.Guard;

namespace ProForma.Common.Guard;

public class Guard<T> : IGuard<T>
    where T : Exception, new()
{
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
```
Now you remember the `if`'s from before? Here is how they rewrite with the `Guard<T>`, not really shorter, but in opinion much more readable. 
```CS
Guard<DirectoryNotFoundException>.IsFalse(Directory.Exists(physicalPath))
    .Throw($"Could not find the given path '{physicalPath}'.");
Guard<DuplicateNameException>.IsTrue(_directories.ContainsKey(urlPrefix))
	.Throw($"Key '{urlPrefix}' already exists.");
Guard<Exception>.IsTrue(_directories.ContainsValue(physicalPath))
	.Throw($"Physical Path '{physicalPath}' already exists.");
```
While debugging, I encountered a little mistake I made.
When you write:
```CS
Guard<ArgumentOutOfRangeException>.IsLessThanOrEqualTo(startPort + portRange, ushort.MaxValue)
    .Throw($"The given range of {portRange} plus the start port of {startPort} exceeds the maximum range of {ushort.MaxValue}");
```
You would expect that the exception is thrown when `startPort + portRange` is less than `ushort.MaxValue` but it seems I made it all the other way round... when you have not met the condition, the exception is fired - will put it on my to do for corrections. 

Hope you liked it so far, and I promise there will be some cooler parts coming like the plugin system or the communication part between C# and JS/TS. 

Greetings
Marlene