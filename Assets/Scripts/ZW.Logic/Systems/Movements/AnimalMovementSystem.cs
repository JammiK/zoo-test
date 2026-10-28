using System.Linq;
using Core.JG.Data.Interfaces.Services;
using JG.Data.Entities.Interfaces.Repositories.Common;
using UnityEngine;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Managers.Animals;
using ZW.Data.Interfaces.Providers.Animals;
using ZW.Logic.Interfaces.Services.Levels;
using ZW.Logic.Interfaces.Services.Movements;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Logic.Interfaces.Systems.Movements;
using ZW.Logic.Models.Movements;

namespace ZW.Logic.Systems.Movements
{
    public class AnimalMovementSystem : IAnimalMovementSystem
    {
        private readonly IAnimalMovementStrategy[] _strategies;
        private readonly IAnimalProvider _animalProvider;
        private readonly IAnimalManager _animalManager;
        private readonly ILevelAreaService _levelAreaService;
        private readonly IAreaReturnService _areaReturnService;
        private readonly IRandomService _randomService;
        private readonly IAnimalMotionStream _motionStream;

        public AnimalMovementSystem(
            IAnimalMovementStrategy[] strategies,
            IEntityRepository<IAnimalEntity> animalRepository,
            IAnimalProvider animalProvider,
            IAnimalManager animalManager,
            ILevelAreaService levelAreaService,
            IAreaReturnService areaReturnService,
            IRandomService randomService,
            IAnimalMotionStream motionStream)
        {
            _strategies = strategies;
            _animalProvider = animalProvider;
            _animalManager = animalManager;
            _levelAreaService = levelAreaService;
            _areaReturnService = areaReturnService;
            _randomService = randomService;
            _motionStream = motionStream;

            foreach (var entity in animalRepository.GetAll())
            {
                var count = _strategies.Count(strategy => strategy.CanMove(entity));
                if (count != 1)
                {
                    Debug.LogError($"Animal '{entity.Id}' must have exactly one movement config, found {count}");
                }
            }
        }

        public void Tick(float deltaTime)
        {
            foreach (var animal in _animalProvider.GetAlive().ToList())
            {
                var strategy = _strategies.FirstOrDefault(s => s.CanMove(animal.Entity));
                if (strategy == null)
                {
                    continue;
                }

                var isInside = _levelAreaService.IsInside(animal.Position);
                var returnDirection = isInside
                    ? animal.Direction
                    : _areaReturnService.GetReturnDirection(animal.Position, animal.Direction, _randomService.NextFloat());
                var context = new MovementContext(deltaTime, isInside, returnDirection);

                var decision = strategy.Decide(animal, context);
                _animalManager.SetDirection(animal, decision.Direction);
                _animalManager.SetMoveCooldown(animal, decision.Cooldown);

                if (decision.Motion.HasValue)
                {
                    _motionStream.OnNext(new AnimalMotionCommand(animal.Uid, decision.Motion.Value));
                }
            }
        }
    }
}
