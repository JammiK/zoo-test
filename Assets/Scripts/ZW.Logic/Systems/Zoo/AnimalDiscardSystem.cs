using UnityEngine;
using ZW.Data.Interfaces.Managers.Zoo;
using ZW.Data.Interfaces.Providers.Animals;
using ZW.Logic.Interfaces.Systems.Zoo;

namespace ZW.Logic.Systems.Zoo
{
    public class AnimalDiscardSystem : IAnimalDiscardSystem
    {
        private readonly IAnimalProvider _animalProvider;
        private readonly IZooManager _zooManager;

        public AnimalDiscardSystem(IAnimalProvider animalProvider, IZooManager zooManager)
        {
            _animalProvider = animalProvider;
            _zooManager = zooManager;
        }

        public void Discard(int uid)
        {
            var animal = _animalProvider.Get(uid);
            if (animal == null)
            {
                return;
            }

            _zooManager.RemoveAnimal(animal);
            Debug.LogWarning($"Discarded animal {uid} ('{animal.Entity?.Id}'): its view could not be created");
        }
    }
}
