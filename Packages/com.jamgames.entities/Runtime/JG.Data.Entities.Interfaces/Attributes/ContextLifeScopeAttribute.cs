using System;

namespace JG.Data.Entities.Interfaces.Attributes
{
    public enum ContextLifeScope
    {
        Global,
        Scene,
    }
    
    [AttributeUsage(AttributeTargets.Interface)]
    public class ContextLifeScopeAttribute : Attribute
    {
        public ContextLifeScope scope;
        public ContextLifeScopeAttribute(ContextLifeScope scope)
        {
            this.scope = scope;
        }
    }
}