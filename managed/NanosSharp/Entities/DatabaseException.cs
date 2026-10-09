namespace NanosSharp.Entities;

/// <summary>Raised when a database query fails.</summary>
public sealed class DatabaseException(string message) : Exception(message);
