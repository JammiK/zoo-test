using System;
using System.Reflection;

namespace JG.Data.Entities.Interfaces.Attributes
{
    public static class AbstractEntityExtensions
    {
        public static bool IsAbstractEntity(this Type type) => type.GetCustomAttribute<AbstractEntityAttribute>() != null;
    }
}
