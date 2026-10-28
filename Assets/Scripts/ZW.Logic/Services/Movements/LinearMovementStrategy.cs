using System.Collections.Generic;
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
        private readonly Dictionary<string, ILinearMovementEntity> _configs;

        public LinearMovementStrategy(IEntityRepository<ILinearMovementEntity> configRepository)
        {
            _configs = configRepository.IndexByAnimal();
        }

        public bool CanMove(IAnimalEntity entity)
        {
            return entity != null && _configs.ContainsKey(entity.Id);
        }

        public MovementDecision Decide(IAnimalModel animal, MovementContext context)
        {
            var config = _configs[animal.Entity.Id];
            var direction = context.IsInsideArea ? animal.Direction : context.ReturnDirection;

            return new MovementDecision(direction, 0f, new AnimalMotion(direction.Scale(config.Speed), 0f));
        }
    }
}
