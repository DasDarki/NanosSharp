local function bench(n)
  local x = 0x9E3779B97F4A7C15
  local acc = 0
  for _ = 1, n do
    x = x ~ (x >> 12)
    x = x ~ (x << 25)
    x = x ~ (x >> 27)
    acc = acc + x * 0x2545F4914F6CDD1D
  end
  return acc
end

local n = tonumber(arg[1]) or 20000000
bench(1000)
local t0 = os.clock()
local r = bench(n)
local t1 = os.clock()
io.write(string.format("result=%016x elapsed_ms=%.1f\n", r, (t1 - t0) * 1000))
