namespace NanosSharp.Tests.Framework;

public static class Assert
{
    public static void True(bool condition, string message)
    {
        if (!condition)
        {
            throw new AssertionException(message);
        }
    }

    public static void Equal<T>(T expected, T actual, string? message = null)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new AssertionException($"{message ?? "Values differ"}: expected '{expected}', actual '{actual}'");
        }
    }

    public static TException Throws<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException exception)
        {
            return exception;
        }
        catch (Exception exception)
        {
            throw new AssertionException($"Expected {typeof(TException).Name}, got {exception.GetType().Name}: {exception.Message}");
        }

        throw new AssertionException($"Expected {typeof(TException).Name}, nothing was thrown");
    }
}
