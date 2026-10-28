using System;
using System.Collections.Generic;
using Core.JG.Data.Interfaces.Services;
using JG.Data.Entities.Interfaces.Repositories.Common;
using UnityEngine;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Managers.Animals;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Interfaces.Providers.Animals;
using ZW.Data.Interfaces.Types.Animals;
using ZW.Logic.Interfaces.Services.Levels;
using ZW.Logic.Interfaces.Services.Movements;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Logic.Interfaces.Systems.Movements;
using ZW.Logic.Models.Movements;

namespace ZW.Logic.Systems.Movements
{
    public class AnimalMovementSystem : IAnimalMovementSystem
    {
        private readonly Dictionary<string, IAnimalMovementStrategy> _strategyByAnimal = new();
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
            _animalProvider = animalProvider;
            _animalManager = animalManager;
            _levelAreaService = levelAreaService;
            _areaReturnService = areaReturnService;
            _randomService = randomService;
            _motionStream = motionStream;

            foreach (var entity in animalRepository.GetAll())
            {
                var count = 0;
                foreach (var strategy in strategies)
                {
                    if (!strategy.CanMove(entity))
                    {
                        continue;
                    }

                    count++;
                    _strategyByAnimal.TryAdd(entity.Id, strategy);
                }

                if (count != 1)
                {
                    Debug.LogError($"Animal '{entity.Id}' must have exactly one movement config, found {count}");
                }
            }
        }

        public void Tick(float deltaTime)
        {
            var animals = _animalProvider.GetAll();
            for (var i = 0; i < animals.Count; i++)
            {
                var animal = animals[i];
                if (animal.State != AnimalState.Alive
                    || animal.Entity == null
                    || !_strategyByAnimal.TryGetValue(animal.Entity.Id, out var strategy))
                {
                    continue;
                }

                try
                {
                    Move(animal, strategy, deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private void Move(IAnimalModel animal, IAnimalMovementStrategy strategy, float deltaTime)
        {
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
                if (animal.KeepDirection)
                {
                    _animalManager.SetKeepDirection(animal, false);
                }

                _motionStream.OnNext(new AnimalMotionCommand(animal.Uid, decision.Motion.Value));
            }
        }
    }
}
