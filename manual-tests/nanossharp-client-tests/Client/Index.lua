Events.SubscribeRemote("nanossharp:server-ping", function(value)
    Events.CallRemote("nanossharp:client-pong", value)
end)

Package.Subscribe("Load", function()
    Events.CallRemote("nanossharp:client-ready", 42, "hello from lua", Vector(1, 2, 3))
end)
