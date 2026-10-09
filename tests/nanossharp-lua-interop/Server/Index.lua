Package.Export("LuaInteropLibrary", {
    Add = function(first, second)
        return first + second
    end,
    Greet = function(name)
        return "hello " .. name
    end,
    Invoke = function(callback, value)
        return callback(value)
    end,
})

Package.Export("LuaInteropUseCSharp", function(first, second)
    return CSharpInteropLibrary.Multiply(first, second)
end)

Package.Export("LuaInteropUseCSharpFunction", function(text)
    return NanosSharpShout(text)
end)
