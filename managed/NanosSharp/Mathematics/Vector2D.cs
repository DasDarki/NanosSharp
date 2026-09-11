namespace NanosSharp.Mathematics;

/// <summary>A 2D vector.</summary>
public readonly record struct Vector2D(double X, double Y)
{
    /// <summary>The zero vector.</summary>
    public static Vector2D Zero => default;

    /// <inheritdoc />
    public override string ToString() => $"Vector2D({X}, {Y})";
}
