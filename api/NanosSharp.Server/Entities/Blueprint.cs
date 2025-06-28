using NanosSharp.API;

namespace NanosSharp.Server;

public class Blueprint(LuaRef handle) : NanosUnit(handle), IBaseEntity, IBaseActor, IBasePaintable
{
    
}