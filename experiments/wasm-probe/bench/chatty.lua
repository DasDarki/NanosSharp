local probe = require("probe")
local nop = probe.nop

local function bench(m)
  local acc = 0
  for _ = 1, m do
    acc = nop(acc)
  end
  return acc
end

local m = tonumber(arg[1]) or 50000000
bench(1000)
local t0 = os.clock()
local r = bench(m)
local t1 = os.clock()
io.write(string.format("result=%d elapsed_ms=%.1f\n", r, (t1 - t0) * 1000))
