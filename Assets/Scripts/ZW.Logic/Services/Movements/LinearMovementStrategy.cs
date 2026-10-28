using ZW.Data.Interfaces.Entities.Movements;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Extensions;
using ZW.Logic.Models.Movements;

namespace ZW.Logic.Services.Movements
{
    public class LinearMovementStrategy : MovementStrategyBase<ILinearMovementEntity>
    {
        protected override MovementDecision Decide(IAnimalModel animal, ILinearMovementEntity config, MovementContext context)
        {
            var direction = context.IsInsideArea ? animal.Direction : context.ReturnDirection;

            return new MovementDecision(direction, 0f, new AnimalMotion(direction.Scale(config.Speed), 0f));
        }
    }
}
