local callHandlers = {}

Bridge = WebUI("NanoSharp Bridge", "file://bridge/index.html")

function onCall(eventName, callback)
    callHandlers[eventName] = callback
end

function callEvent(eventName, ...)
    Bridge:CallEvent("nanos-bridge:call-event", eventName, ...)
end

function callRPC(callID, returnValue)
    Bridge:CallEvent("nanos-bridge:call-rpc", callID, returnValue)
end

Bridge:Subscribe("nanos-bridge:call", function (msg)
    local data = JSON.parse(msg)
    local key = data.key
    local args = data.args
    local callID = data['callID']
    local cb = callHandlers[key]
    
    if cb ~= nil then
        local result = cb(table.unpack(args))
        if callID ~= nil then
            callRPC(callID, result)
        end
    end
end)

local function initPossiblePackage(name)
    if #File.GetFiles("../" .. name .. "Client/", ".cs") > 0 then
        callEvent("@bridge:package:load", File.GetFullPath("../" .. name .. "Client/"))
    end
end

Package.Subscribe("Load", function ()
    local packages = File.GetDirectories("../")

    for _, package in pairs(packages) do
        if not string.match(package, ".transient") and not string.match(package, ".cache") and not string.match(package, ".data") then
            if File.Exists("../" .. package .. "/Client/") then
                print("Checking package: " .. package)
    
                initPossiblePackage(package)
            end
        end
    end
end)

onCall("print", function (msg)
    print(msg)
end)