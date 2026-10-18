#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Create custom keybindings and retrieve input information.</summary>
public static partial class Input
{
    /// <summary>A key binding has been changed through the settings</summary>
    public static event KeyBindingChangeHandler? OnKeyBindingChange
    {
        add => KeyBindingChangeEvent.Add(value);
        remove => KeyBindingChangeEvent.Remove(value);
    }

    /// <summary>A keyboard key is being pressed</summary>
    public static event KeyDownHandler? OnKeyDown
    {
        add => KeyDownEvent.Add(value);
        remove => KeyDownEvent.Remove(value);
    }

    /// <summary>A keyboard key has been pressed</summary>
    public static event KeyPressHandler? OnKeyPress
    {
        add => KeyPressEvent.Add(value);
        remove => KeyPressEvent.Remove(value);
    }

    /// <summary>A keyboard key has been released</summary>
    public static event KeyUpHandler? OnKeyUp
    {
        add => KeyUpEvent.Add(value);
        remove => KeyUpEvent.Remove(value);
    }

    /// <summary>A mouse button has been pressed / is being pressed</summary>
    public static event MouseDownHandler? OnMouseDown
    {
        add => MouseDownEvent.Add(value);
        remove => MouseDownEvent.Remove(value);
    }

    /// <summary>When mouse cursor is displayed/hidden</summary>
    public static event MouseEnableHandler? OnMouseEnable
    {
        add => MouseEnableEvent.Add(value);
        remove => MouseEnableEvent.Remove(value);
    }

    /// <summary>Called when the mouse moves</summary>
    public static event MouseMoveHandler? OnMouseMove
    {
        add => MouseMoveEvent.Add(value);
        remove => MouseMoveEvent.Remove(value);
    }

    /// <summary>Called when the mouse scrolls</summary>
    public static event MouseScrollHandler? OnMouseScroll
    {
        add => MouseScrollEvent.Add(value);
        remove => MouseScrollEvent.Remove(value);
    }

    /// <summary>A mouse button has been released</summary>
    public static event MouseUpHandler? OnMouseUp
    {
        add => MouseUpEvent.Add(value);
        remove => MouseUpEvent.Remove(value);
    }

    private static readonly ClassEvent<KeyBindingChangeHandler> KeyBindingChangeEvent = new("Input_S", "KeyBindingChange");
    private static readonly ClassEvent<KeyDownHandler> KeyDownEvent = new("Input_S", "KeyDown");
    private static readonly ClassEvent<KeyPressHandler> KeyPressEvent = new("Input_S", "KeyPress");
    private static readonly ClassEvent<KeyUpHandler> KeyUpEvent = new("Input_S", "KeyUp");
    private static readonly ClassEvent<MouseDownHandler> MouseDownEvent = new("Input_S", "MouseDown");
    private static readonly ClassEvent<MouseEnableHandler> MouseEnableEvent = new("Input_S", "MouseEnable");
    private static readonly ClassEvent<MouseMoveHandler> MouseMoveEvent = new("Input_S", "MouseMove");
    private static readonly ClassEvent<MouseScrollHandler> MouseScrollEvent = new("Input_S", "MouseScroll");
    private static readonly ClassEvent<MouseUpHandler> MouseUpEvent = new("Input_S", "MouseUp");
    private static readonly StaticMethodSlot GetGameKeyBindingsSlot = new("Input_S", "GetGameKeyBindings");
    private static readonly StaticMethodSlot GetKeyCodeSlot = new("Input_S", "GetKeyCode");
    private static readonly StaticMethodSlot GetKeyIconSlot = new("Input_S", "GetKeyIcon");
    private static readonly StaticMethodSlot GetKeyboardLayoutSlot = new("Input_S", "GetKeyboardLayout");
    private static readonly StaticMethodSlot GetModifierKeysSlot = new("Input_S", "GetModifierKeys");
    private static readonly StaticMethodSlot GetMouseCursorSlot = new("Input_S", "GetMouseCursor");
    private static readonly StaticMethodSlot GetScriptingKeyBindingsSlot = new("Input_S", "GetScriptingKeyBindings");
    private static readonly StaticMethodSlot InputKeySlot = new("Input_S", "InputKey");
    private static readonly StaticMethodSlot IsInputEnabledSlot = new("Input_S", "IsInputEnabled");
    private static readonly StaticMethodSlot IsKeyDownSlot = new("Input_S", "IsKeyDown");
    private static readonly StaticMethodSlot IsMouseEnabledSlot = new("Input_S", "IsMouseEnabled");
    private static readonly StaticMethodSlot SetInputEnabledSlot = new("Input_S", "SetInputEnabled");
    private static readonly StaticMethodSlot SetMouseCursorSlot = new("Input_S", "SetMouseCursor");
    private static readonly StaticMethodSlot SetMouseEnabledSlot = new("Input_S", "SetMouseEnabled");

    /// <summary>Returns a table with all Game KeyBindings</summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> GameKeyBindings => GetGameKeyBindings();

    /// <summary>Gets the keyboard layout of the user</summary>
    public static KeyboardLayout KeyboardLayout => GetKeyboardLayout();

    /// <summary>Gets the currently pressed modifier keys</summary>
    public static KeyModifier ModifierKeys => GetModifierKeys();

    /// <summary>Gets the current Mouse Cursor type</summary>
    public static CursorType MouseCursor
    {
        get => GetMouseCursor();
        set => SetMouseCursor(value);
    }

    /// <summary>Returns a table with all Scripting KeyBindings</summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> ScriptingKeyBindings => GetScriptingKeyBindings();

    /// <summary>Gets if the game input is enabled</summary>
    public static bool InputEnabled
    {
        get => IsInputEnabled();
        set => SetInputEnabled(value);
    }

    /// <summary>Gets if the mouse cursor is enabled</summary>
    public static bool MouseEnabled
    {
        get => IsMouseEnabled();
        set => SetMouseEnabled(value);
    }

    /// <summary>Returns a table with all Game KeyBindings</summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> GetGameKeyBindings() =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(GetGameKeyBindingsSlot, 0)!;

    /// <summary>Gets the key code of a key</summary>
    public static byte GetKeyCode(string keyName) => ScriptApi.CallResult<byte, string>(GetKeyCodeSlot, 1, keyName)!;

    /// <summary>Gets the icon path of a key</summary>
    public static string GetKeyIcon(string keyName, bool? darkMode = null) =>
        ScriptApi.CallResult<string, string, bool?>(GetKeyIconSlot, 1, keyName, darkMode)!;

    /// <summary>Gets the keyboard layout of the user</summary>
    public static KeyboardLayout GetKeyboardLayout() => ScriptApi.CallResult<KeyboardLayout>(GetKeyboardLayoutSlot, 0)!;

    /// <summary>Gets the currently pressed modifier keys</summary>
    public static KeyModifier GetModifierKeys() => ScriptApi.CallResult<KeyModifier>(GetModifierKeysSlot, 0)!;

    /// <summary>Gets the current Mouse Cursor type</summary>
    public static CursorType GetMouseCursor() => ScriptApi.CallResult<CursorType>(GetMouseCursorSlot, 0)!;

    /// <summary>Returns a table with all Scripting KeyBindings</summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> GetScriptingKeyBindings() =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(GetScriptingKeyBindingsSlot, 0)!;

    /// <summary>Forces an Input Key event on Local Player</summary>
    public static void InputKey(string keyName, InputEvent inputEvent, float? amountDepressed = null) =>
        ScriptApi.Call(InputKeySlot, 2, keyName, inputEvent, amountDepressed);

    /// <summary>Gets if the game input is enabled</summary>
    public static bool IsInputEnabled() => ScriptApi.CallResult<bool>(IsInputEnabledSlot, 0)!;

    /// <summary>Returns if a key is being pressed</summary>
    public static bool IsKeyDown(string keyName) => ScriptApi.CallResult<bool, string>(IsKeyDownSlot, 1, keyName)!;

    /// <summary>Gets if the mouse cursor is enabled</summary>
    public static bool IsMouseEnabled() => ScriptApi.CallResult<bool>(IsMouseEnabledSlot, 0)!;

    /// <summary>Toggles Local Player input</summary>
    public static void SetInputEnabled(bool enableInput) => ScriptApi.Call(SetInputEnabledSlot, 1, enableInput);

    /// <summary>Sets the current Mouse Cursor type</summary>
    public static void SetMouseCursor(CursorType cursorType) => ScriptApi.Call(SetMouseCursorSlot, 1, cursorType);

    /// <summary>Shows or hides the mouse cursor</summary>
    public static void SetMouseEnabled(bool isEnabled) => ScriptApi.Call(SetMouseEnabledSlot, 1, isEnabled);

    /// <summary>A key binding has been changed through the settings</summary>
    public delegate void KeyBindingChangeHandler(string? bindingName, string? key, float scale);

    /// <summary>A keyboard key is being pressed</summary>
    public delegate bool KeyDownHandler(string? keyName, float delta);

    /// <summary>A keyboard key has been pressed</summary>
    public delegate bool KeyPressHandler(string? keyName, float delta);

    /// <summary>A keyboard key has been released</summary>
    public delegate bool KeyUpHandler(string? keyName, float delta);

    /// <summary>A mouse button has been pressed / is being pressed</summary>
    public delegate bool MouseDownHandler(string? keyName, float mouseX, float mouseY);

    /// <summary>When mouse cursor is displayed/hidden</summary>
    public delegate void MouseEnableHandler(bool isEnabled);

    /// <summary>Called when the mouse moves</summary>
    public delegate bool MouseMoveHandler(float cursorDeltaX, float cursorDeltaY, float mouseX, float mouseY);

    /// <summary>Called when the mouse scrolls</summary>
    public delegate bool MouseScrollHandler(float mouseX, float mouseY, float delta);

    /// <summary>A mouse button has been released</summary>
    public delegate bool MouseUpHandler(string? keyName, float mouseX, float mouseY);
}
