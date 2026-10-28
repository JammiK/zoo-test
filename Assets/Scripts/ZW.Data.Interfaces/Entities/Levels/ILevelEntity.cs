using JG.Data.Entities.Interfaces.Attributes;
using JG.Data.Entities.Interfaces.Entities.Common;
using ZW.Data.Models.Types;

namespace ZW.Data.Interfaces.Entities.Levels
{
    public interface ILevelEntity : IEntity
    {
        [PrefabField] string Prefab { get; }
        PlanarVector Size { get; }
    }
}
