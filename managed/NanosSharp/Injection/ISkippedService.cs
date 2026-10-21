namespace NanosSharp.Injection;

/// <summary>An <see cref="IService"/> that is created and resolved but not started automatically.</summary>
public interface ISkippedService : IService
{
}
