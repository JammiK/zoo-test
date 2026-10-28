using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Entities.Movements;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Interfaces.Services.Movements;
using ZW.Logic.Models.Movements;

namespace ZW.Logic.Services.Movements
{
    public abstract class MovementStrategyBase<TConfig> : IAnimalMovementStrategy
        where TConfig : class, IMovementEntity
    {
        public bool CanMove(IAnimalEntity entity)
        {
            return entity?.Movement is TConfig;
        }

        public MovementDecision Decide(IAnimalModel animal, MovementContext context)
        {
            return Decide(animal, (TConfig)animal.Entity.Movement, context);
        }

        protected abstract MovementDecision Decide(IAnimalModel animal, TConfig config, MovementContext context);
    }
}
