using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Entities.Interfaces.Repositories.Common;
using ZW.Data.Interfaces.Entities.Movements;

namespace ZW.Logic.Extensions
{
    public static class MovementConfigExtensions
    {
        public static Dictionary<string, T> IndexByAnimal<T>(this IEntityRepository<T> repository)
            where T : IEntity, IAnimalMovementConfig
        {
            var index = new Dictionary<string, T>();
            foreach (var config in repository.GetAll())
            {
                foreach (var animal in config.Animals)
                {
                    index.TryAdd(animal.Id, config);
                }
            }

            return index;
        }
    }
}
