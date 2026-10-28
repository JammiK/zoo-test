using System;

namespace JG.Data.Entities.Interfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class CustomEntityFileNameAttribute : Attribute
    {
        public string FileName;
        public CustomEntityFileNameAttribute(string fileName)
        {
            FileName = fileName;
        }
    }
}