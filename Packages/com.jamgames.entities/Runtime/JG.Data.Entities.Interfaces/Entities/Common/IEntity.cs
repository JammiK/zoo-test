using JG.Data.Entities.Interfaces.Attributes;

namespace JG.Data.Entities.Interfaces.Entities.Common
{
    public interface IEntity
    {
        [IgnoreEntityField]
        string Id { get; }
    }
}