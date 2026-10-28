using System;

namespace JG.Data.Entities.Interfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultValueSnippetAttribute : Attribute
    {
        public string Value;
        public DefaultValueSnippetAttribute(string value)
        {
            Value = value;
        }
    }
}