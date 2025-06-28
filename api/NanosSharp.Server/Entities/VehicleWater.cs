using NanosSharp.API;

namespace NanosSharp.Server;

public class VehicleWater(LuaRef handle) : NanosUnit(handle), IBaseEntity, IBaseActor, IBaseVehicle, IBasePaintable, IBaseDamageable
{
    
}