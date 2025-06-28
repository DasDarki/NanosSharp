using NanosSharp.API;

namespace NanosSharp.Server;

public class CharacterSimple(LuaRef handle) : NanosUnit(handle), IBaseEntity, IBaseActor, IBasePawn, IBasePaintable, IBaseDamageable
{
    
}