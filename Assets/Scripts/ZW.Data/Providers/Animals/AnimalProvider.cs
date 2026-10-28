using System.Collections.Generic;
using System.Linq;
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
            return GetAll().Where(animal => animal.State == AnimalState.Alive);
        }

        public IAnimalModel Get(int uid)
        {
            return GetAll().FirstOrDefault(animal => animal.Uid == uid);
        }
    }
}
