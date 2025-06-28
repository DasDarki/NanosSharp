using NanosSharp.API;

namespace NanosSharp.Server;

public class Grenade(LuaRef handle) : NanosUnit(handle), IBaseEntity, IBaseActor, IBasePaintable, IBasePickable
{
    
}