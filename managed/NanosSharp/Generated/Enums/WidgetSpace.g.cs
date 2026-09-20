#nullable enable

namespace NanosSharp.Enums;

/// <summary>Whether a Widget3D is rendered in the world or on the screen</summary>
public enum WidgetSpace
{
    /// <summary>The widget is rendered in the world as mesh, it can be occluded like any other mesh in the world</summary>
    World = 0,
    /// <summary>The widget is rendered in the screen, completely outside of the world, never occluded</summary>
    Screen = 1,
}
