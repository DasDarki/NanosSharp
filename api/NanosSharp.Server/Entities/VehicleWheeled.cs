using NanosSharp.API;

namespace NanosSharp.Server;

public class VehicleWheeled(LuaRef handle) : NanosUnit(handle), IBaseEntity, IBaseActor, IBaseVehicle, IBasePaintable, IBaseDamageable
{
    
}