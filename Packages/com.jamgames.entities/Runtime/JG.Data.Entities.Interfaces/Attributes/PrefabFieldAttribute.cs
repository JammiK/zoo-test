using System;
using UnityEngine;

namespace JG.Data.Entities.Interfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrefabFieldAttribute : Attribute
    {
        public Type Type { get; }

        public PrefabFieldAttribute()
        {
            Type = typeof(GameObject);
        }

        public PrefabFieldAttribute(Type type)
        {
            Type = type;
        }
    }
}