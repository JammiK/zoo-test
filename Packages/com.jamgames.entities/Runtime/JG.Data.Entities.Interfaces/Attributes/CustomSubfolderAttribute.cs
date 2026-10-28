using System;

namespace JG.Data.Entities.Interfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class CustomSubfolderAttribute : Attribute
    {
        public string Subfolder;

        public CustomSubfolderAttribute(string subfolder)
        {
            Subfolder = subfolder;
        }
    }
}