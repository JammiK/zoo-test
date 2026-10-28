using ZW.Data.Interfaces.Managers.Animals;
using ZW.Data.Interfaces.Providers.Animals;
using ZW.Data.Interfaces.Types.Animals;
using ZW.Data.Models.Types;
using ZW.Logic.Interfaces.Systems.Movements;

namespace ZW.Logic.Systems.Movements
{
    public class AnimalPositionSyncSystem : IAnimalPositionSyncSystem
    {
        private readonly IAnimalProvider _animalProvider;
        private readonly IAnimalManager _animalManager;

        public AnimalPositionSyncSystem(IAnimalProvider animalProvider, IAnimalManager animalManager)
        {
            _animalProvider = animalProvider;
            _animalManager = animalManager;
        }

        public void Sync(int uid, PlanarVector position)
        {
            var animal = _animalProvider.Get(uid);
            if (animal == null || animal.State != AnimalState.Alive)
            {
                return;
            }

            _animalManager.SetPosition(animal, position);
        }
    }
}
