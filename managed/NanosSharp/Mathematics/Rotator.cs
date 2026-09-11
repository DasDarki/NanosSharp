namespace NanosSharp.Mathematics;

/// <summary>A rotation in degrees.</summary>
public readonly record struct Rotator(double Pitch, double Yaw, double Roll)
{
    /// <summary>The zero rotation.</summary>
    public static Rotator Zero => default;

    /// <inheritdoc />
    public override string ToString() => $"Rotator({Pitch}, {Yaw}, {Roll})";
}
