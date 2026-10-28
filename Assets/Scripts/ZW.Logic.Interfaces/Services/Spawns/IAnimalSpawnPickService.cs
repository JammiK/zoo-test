using System.Collections.Generic;
using JetBrains.Annotations;
using ZW.Data.Interfaces.Entities.Animals;

namespace ZW.Logic.Interfaces.Services.Spawns
{
    public interface IAnimalSpawnPickService
    {
        [CanBeNull] IAnimalEntity Pick(IEnumerable<IAnimalEntity> animals, float roll);
    }
}
