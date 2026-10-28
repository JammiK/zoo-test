using System.Linq;
using JG.Data.Entities.Interfaces.Repositories.Common;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Entities.Movements;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Extensions;
using ZW.Logic.Interfaces.Services.Movements;
using ZW.Logic.Models.Movements;

namespace ZW.Logic.Services.Movements
{
    public class LinearMovementStrategy : IAnimalMovementStrategy
    {
        private readonly IEntityRepository<ILinearMovementEntity> _configRepository;

        public LinearMovementStrategy(IEntityRepository<ILinearMovementEntity> configRepository)
        {
            _configRepository = configRepository;
        }

        public bool CanMove(IAnimalEntity entity)
        {
            return entity != null && GetConfig(entity) != null;
        }

        public MovementDecision Decide(IAnimalModel animal, MovementContext context)
        {
            var config = GetConfig(animal.Entity);
            var direction = context.IsInsideArea ? animal.Direction : context.ReturnDirection;

            return new MovementDecision(direction, 0f, new AnimalMotion(direction.Scale(config.Speed), 0f));
        }

        private ILinearMovementEntity GetConfig(IAnimalEntity entity)
        {
            return _configRepository.GetFirst(config => config.Animals.Any(animal => animal.Id == entity.Id));
        }
    }
}
