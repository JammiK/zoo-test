using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Providers.Contexts;
using ZW.Data.Interfaces.Contexts;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Interfaces.Providers.Animals;
using ZW.Data.Interfaces.Types.Animals;

namespace ZW.Data.Providers.Animals
{
    public class AnimalProvider : IAnimalProvider
    {
        private readonly IContextProvider _contextProvider;

        public AnimalProvider(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public IReadOnlyList<IAnimalModel> GetAll()
        {
            return _contextProvider.Get<IZooContext>().Animals;
        }

        public IEnumerable<IAnimalModel> GetAlive()
        {
            foreach (var animal in GetAll())
            {
                if (animal.State == AnimalState.Alive)
                {
                    yield return animal;
                }
            }
        }

        public IAnimalModel Get(int uid)
        {
            return _contextProvider.Get<IZooContext>().AnimalsByUid.TryGetValue(uid, out var animal) ? animal : null;
        }
    }
}
