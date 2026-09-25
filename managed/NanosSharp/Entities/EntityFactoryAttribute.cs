namespace NanosSharp.Entities;

/// <summary>Makes nanos world wrap every entity of the base scripting class with this class, registered automatically.</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EntityFactoryAttribute : Attribute;
