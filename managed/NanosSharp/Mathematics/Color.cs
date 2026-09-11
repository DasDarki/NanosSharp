namespace NanosSharp.Mathematics;

/// <summary>A linear color with components from 0 to 1.</summary>
public readonly record struct Color(float R, float G, float B, float A = 1f)
{
    public static Color White => new(1f, 1f, 1f);

    public static Color Black => new(0f, 0f, 0f);

    public static Color Red => new(1f, 0f, 0f);

    public static Color Green => new(0f, 1f, 0f);

    public static Color Blue => new(0f, 0f, 1f);

    /// <inheritdoc />
    public override string ToString() => $"Color({R}, {G}, {B}, {A})";
}
