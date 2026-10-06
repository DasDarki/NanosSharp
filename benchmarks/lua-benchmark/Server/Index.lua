local iterations = 1000000
local warmup_seconds = 0.5

local function measure(name, operation)
    local warmup_started = os.clock()
    while os.clock() - warmup_started < warmup_seconds do
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

    local started = os.clock()
    local limit = 2000000
    local composite = {}
    local primes = 0
    for number = 2, limit do
        if not composite[number] then
            primes = primes + 1
            for multiple = number * number, limit, number do
                composite[multiple] = true
            end
        end
    end
    Console.Log(string.format("[BENCH] lua compute prime sieve: %.1f ms (%d primes)", (os.clock() - started) * 1000, primes))

    started = os.clock()
    local values = {}
    local seed = 12345
    for index = 1, 200000 do
        seed = (seed * 1103515245 + 12345) % 2147483648
        values[index] = seed
    end
    table.sort(values)
    Console.Log(string.format("[BENCH] lua compute sort: %.1f ms", (os.clock() - started) * 1000))
end)
