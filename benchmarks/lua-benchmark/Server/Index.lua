local iterations = 20000

local function measure(name, operation)
    for _ = 1, 1000 do
        operation()
    end

    local started = os.clock()
    for _ = 1, iterations do
        operation()
    end

    local elapsed = os.clock() - started
    Console.Log(string.format("[BENCH] lua %s: %.0f ns/op", name, elapsed / iterations * 1e9))
end

Server.Subscribe("Start", function()
    local prop = Prop(Vector(0, 0, 100), Rotator(0, 0, 0), "nanos-world::SM_Cube")
    local location = Vector(10, 20, 30)
    Events.Subscribe("lua-benchmark", function(number, text) end)

    measure("static getter", function() Server.GetTickRate() end)
    measure("entity getter", function() prop:GetLocation() end)
    measure("entity setter", function() prop:SetLocation(location) end)
    measure("event roundtrip", function() Events.Call("lua-benchmark", 1, "text") end)

    prop:Destroy()
end)
