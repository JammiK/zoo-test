using System;
using System.Linq;

namespace Core.JG.Extensions
{
    public static class EnumExtensions
    {
        public static string GetIconAddress(this Enum value)
        {
            var type = value.GetType().FullName.Split('.').Last();
            return $"{type}.{value}.icon";
        }

        public static string GetNameResource(this Enum value)
        {
            var type = value.GetType().FullName.Split('.').Last();
            return $"{type}.{value}.name";
        }
    }
}