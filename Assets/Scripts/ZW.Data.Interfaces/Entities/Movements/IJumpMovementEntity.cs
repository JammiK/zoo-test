using JG.Data.Entities.Interfaces.Entities.Common;

namespace ZW.Data.Interfaces.Entities.Movements
{
    public interface IJumpMovementEntity : IEntity, IAnimalMovementConfig
    {
        float Distance { get; }
        float Height { get; }
        float Interval { get; }
    }
}
