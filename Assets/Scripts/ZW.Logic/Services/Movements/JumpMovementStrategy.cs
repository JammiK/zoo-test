using System;
using ZW.Data.Interfaces.Entities.Movements;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Extensions;
using ZW.Logic.Interfaces.Services.Environment;
using ZW.Logic.Interfaces.Services.Movements;
using ZW.Logic.Models.Movements;

namespace ZW.Logic.Services.Movements
{
    public class JumpMovementStrategy : MovementStrategyBase<IJumpMovementEntity>
    {
        private const float MinHeight = 0.01f;
        private const float MinGravity = 0.01f;

        private readonly IRandomDirectionService _randomDirectionService;
        private readonly IEnvironmentService _environmentService;

        public JumpMovementStrategy(IRandomDirectionService randomDirectionService, IEnvironmentService environmentService)
        {
            _randomDirectionService = randomDirectionService;
            _environmentService = environmentService;
        }

        protected override MovementDecision Decide(IAnimalModel animal, IJumpMovementEntity config, MovementContext context)
        {
            var cooldown = animal.MoveCooldown - context.DeltaTime;
            if (cooldown > 0f)
            {
                return new MovementDecision(animal.Direction, cooldown, null);
            }

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
