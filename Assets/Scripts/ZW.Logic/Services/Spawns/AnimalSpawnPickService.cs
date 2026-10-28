using System;
using System.Collections.Generic;
using System.Linq;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Logic.Interfaces.Services.Spawns;

namespace ZW.Logic.Services.Spawns
{
    public class AnimalSpawnPickService : IAnimalSpawnPickService
    {
        public IAnimalEntity Pick(IEnumerable<IAnimalEntity> animals, float roll)
        {
            var candidates = animals.Where(animal => animal.SpawnWeight > 0f).ToList();
            var target = candidates.Sum(animal => animal.SpawnWeight) * Math.Clamp(roll, 0f, 1f);

            foreach (var animal in candidates)
            {
                target -= animal.SpawnWeight;
                if (target < 0f)
                {
                    return animal;
                }
            }

            return candidates.LastOrDefault();
        }
    }
}
