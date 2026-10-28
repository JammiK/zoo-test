using System;

namespace JG.Data.Entities.Interfaces.Attributes
{
    // Base entity interface for polymorphic links: no Entity/EditorEntity/repository is generated for it.
    [AttributeUsage(AttributeTargets.Interface)]
    public class AbstractEntityAttribute : Attribute
    {
    }
}
