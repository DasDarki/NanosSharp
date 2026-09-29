Events.SubscribeRemote("nanossharp:server-ping", function(value)
    Events.CallRemote("nanossharp:client-ready", "hello from lua", 42, Vector(1, 2, 3))
    Events.CallRemote("nanossharp:client-pong", "pong", value)
end)
