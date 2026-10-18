if not Client.GetValue("NanosSharp") then
    Events.SubscribeRemote("nanossharp:handshake-request", function()
        Events.CallRemote("nanossharp:handshake")
    end)
end

Events.SubscribeRemote("nanossharp:server-ping", function(value)
    Events.CallRemote("nanossharp:client-ready", nil, "hello from lua", 42, Vector(1, 2, 3))
    Events.CallRemote("nanossharp:client-pong", nil, "pong", value)
end)

Package.Export("ClientLuaLibrary", {
    Add = function(first, second)
        return first + second
    end,
    Invoke = function(callback, value)
        return callback(value)
    end,
})

Package.Export("ClientLuaUseCSharp", function(first, second)
    return CSharpClientLibrary.Multiply(first, second)
end)
