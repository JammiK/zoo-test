using JG.Data.Entities.Interfaces.Entities.Common;

namespace ZW.Data.Interfaces.Entities.Movements
{
    public interface ILinearMovementEntity : IEntity, IAnimalMovementConfig
    {
        float Speed { get; }
        float TurnInterval { get; }
    }
}
