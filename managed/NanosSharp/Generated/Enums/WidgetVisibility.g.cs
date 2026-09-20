#nullable enable

namespace NanosSharp.Enums;

/// <summary>Visibility of a WebUI or Widget</summary>
public enum WidgetVisibility
{
    /// <summary>Not rendered and not interactable</summary>
    Hidden = 0,
    /// <summary>Rendered and interactable</summary>
    Visible = 1,
    /// <summary>Rendered but ignores the mouse</summary>
    VisibleNotHitTestable = 2,
}
