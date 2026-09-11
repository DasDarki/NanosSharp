namespace NanosSharp.Mathematics;

/// <summary>A 3D vector in Unreal units.</summary>
public readonly record struct Vector(double X, double Y, double Z)
{
    /// <summary>The zero vector.</summary>
    public static Vector Zero => default;

    /// <summary>The length of the vector.</summary>
    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

    public static Vector operator +(Vector left, Vector right) => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Vector operator -(Vector left, Vector right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Vector operator *(Vector vector, double scale) => new(vector.X * scale, vector.Y * scale, vector.Z * scale);

    /// <summary>Distance between two vectors.</summary>
    public static double Distance(Vector left, Vector right) => (left - right).Length;

    /// <inheritdoc />
    public override string ToString() => $"Vector({X}, {Y}, {Z})";
}
