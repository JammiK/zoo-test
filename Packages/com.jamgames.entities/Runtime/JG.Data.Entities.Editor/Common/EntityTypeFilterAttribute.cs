using System;
using UnityEngine;

namespace JG.Data.Entities.Editor.Common
{
    public class EntityTypeFilterAttribute : PropertyAttribute
    {
        public readonly Type EntityType;

        public EntityTypeFilterAttribute(Type entityType)
        {
            EntityType = entityType;
        }
    }
}
