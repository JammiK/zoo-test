using System;

namespace JG.Data.Entities.Interfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface)]
    public class IgnoreSaveAttribute : Attribute
    {
        
    }
}