﻿namespace NanosSharp.API;

/// <summary>
/// The types for Lua.
/// </summary>
public enum LuaType
{
    None = -1,
    Nil = 0,
    Boolean = 1,
    LightUserData = 2,
    Number = 3,
    String = 4,
    Table = 5,
    Function = 6,
    UserData = 7,
    Thread = 8,
    NumTypes = 9
}