using System;
using System.Linq;
using JG.Data.Entities.Interfaces.Repositories.Common;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Entities.Movements;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Extensions;
using ZW.Logic.Interfaces.Services.Environment;
using ZW.Logic.Interfaces.Services.Movements;
using ZW.Logic.Models.Movements;

namespace ZW.Logic.Services.Movements
{
    public class JumpMovementStrategy : IAnimalMovementStrategy
    {
        private const float MinHeight = 0.01f;

        private readonly IEntityRepository<IJumpMovementEntity> _configRepository;
        private readonly IRandomDirectionService _randomDirectionService;
        private readonly IEnvironmentService _environmentService;

        public JumpMovementStrategy(
            IEntityRepository<IJumpMovementEntity> configRepository,
            IRandomDirectionService randomDirectionService,
            IEnvironmentService environmentService)
        {
            _configRepository = configRepository;
            _randomDirectionService = randomDirectionService;
            _environmentService = environmentService;
        }

        public bool CanMove(IAnimalEntity entity)
        {
            return GetConfig(entity) != null;
        }

        public MovementDecision Decide(IAnimalModel animal, MovementContext context)
        {
            var cooldown = animal.MoveCooldown - context.DeltaTime;
            if (cooldown > 0f)
            {
                return new MovementDecision(animal.Direction, cooldown, null);
            }

            var config = GetConfig(animal.Entity);
            var direction = context.IsInsideArea ? _randomDirectionService.Next() : context.ReturnDirection;
            var gravity = _environmentService.Gravity;
            var verticalVelocity = (float)Math.Sqrt(2f * gravity * Math.Max(config.Height, MinHeight));
            var airTime = 2f * verticalVelocity / gravity;
            var motion = new AnimalMotion(direction.Scale(config.Distance / airTime), verticalVelocity);

            return new MovementDecision(direction, config.Interval, motion);
        }

        private IJumpMovementEntity GetConfig(IAnimalEntity entity)
        {
            return _configRepository.GetFirst(config => config.Animals.Any(animal => animal.Id == entity.Id));
        }
    }
}
