using System;

namespace JG.Data.Entities.Interfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityLinkAttribute : Attribute
    {
        public Type EntityType;
        public EntityLinkAttribute(Type entityType)
        {
            EntityType = entityType;
        }
    }
}