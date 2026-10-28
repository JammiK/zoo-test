using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Models.Movements;

namespace ZW.Logic.Interfaces.Services.Movements
{
    public interface IAnimalMovementStrategy
    {
        bool CanMove(IAnimalEntity entity);
        MovementDecision Decide(IAnimalModel animal, MovementContext context);
    }
}
