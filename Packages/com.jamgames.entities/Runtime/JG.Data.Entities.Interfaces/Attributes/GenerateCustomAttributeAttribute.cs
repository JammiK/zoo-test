using System;

namespace JG.Data.Entities.Interfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GenerateCustomAttributeAttribute : Attribute
    {
        public Type Attribute;
        public object[] Parameters;
        public GenerateCustomAttributeAttribute(Type attribute, params object[] args)
        {
            Attribute = attribute;
            Parameters = args;
        }
    }
}