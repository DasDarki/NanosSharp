using NanosSharp.API;

namespace NanosSharp.Server;

public class Weapon(LuaRef handle) : NanosUnit(handle), IBaseEntity, IBaseActor, IBasePaintable, IBasePickable
{
    
}