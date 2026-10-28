using ZW.Data.Interfaces.Entities.Animals;

namespace ZW.Data.Interfaces.Entities.Movements
{
    public interface IAnimalMovementConfig
    {
        IAnimalEntity[] Animals { get; }
    }
}
