using System;
using System.Collections.Generic;
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
        private const float MinGravity = 0.01f;

        private readonly Dictionary<string, IJumpMovementEntity> _configs;
        private readonly IRandomDirectionService _randomDirectionService;
        private readonly IEnvironmentService _environmentService;

        public JumpMovementStrategy(
            IEntityRepository<IJumpMovementEntity> configRepository,
            IRandomDirectionService randomDirectionService,
            IEnvironmentService environmentService)
        {
            _configs = configRepository.IndexByAnimal();
            _randomDirectionService = randomDirectionService;
            _environmentService = environmentService;
        }

        public bool CanMove(IAnimalEntity entity)
        {
            return entity != null && _configs.ContainsKey(entity.Id);
        }

        public MovementDecision Decide(IAnimalModel animal, MovementContext context)
        {
            var cooldown = animal.MoveCooldown - context.DeltaTime;
            if (cooldown > 0f)
            {
                return new MovementDecision(animal.Direction, cooldown, null);
            }

            var config = _configs[animal.Entity.Id];
            var direction = !context.IsInsideArea ? context.ReturnDirection
                : animal.KeepDirection ? animal.Direction
                : _randomDirectionService.Next();
            var gravity = _environmentService.Gravity;
            var verticalVelocity = 0f;
            var airTime = 1f;
            if (gravity > MinGravity)
            {
                verticalVelocity = (float)Math.Sqrt(2f * gravity * Math.Max(config.Height, MinHeight));
                airTime = 2f * verticalVelocity / gravity;
            }

            var motion = new AnimalMotion(direction.Scale(config.Distance / airTime), verticalVelocity);

            return new MovementDecision(direction, config.Interval, motion);
        }
    }
}
