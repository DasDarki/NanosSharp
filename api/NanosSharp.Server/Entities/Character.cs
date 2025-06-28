using NanosSharp.API;

namespace NanosSharp.Server;

public class Character(LuaRef handle) : NanosUnit(handle), IBaseEntity, IBaseActor, IBasePawn, IBasePaintable, IBaseDamageable
{
    
}