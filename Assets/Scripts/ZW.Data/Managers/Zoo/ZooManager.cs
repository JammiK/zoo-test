using System;
using JG.Data.Entities.Interfaces.Providers.Contexts;
using ZW.Data.Interfaces.Contexts;
using ZW.Data.Interfaces.Managers.Zoo;
using ZW.Data.Interfaces.Models.Animals;

namespace ZW.Data.Managers.Zoo
{
    public class ZooManager : IZooManager
    {
        private readonly IContextProvider _contextProvider;

        public ZooManager(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public int TakeNextUid()
        {
            return _contextProvider.Get<IZooContext>().NextUid++;
        }

        public void AddAnimal(IAnimalModel animal)
        {
            _contextProvider.Get<IZooContext>().Animals.Add(animal);
        }

        public void RemoveAnimal(IAnimalModel animal)
        {
            _contextProvider.Get<IZooContext>().Animals.Remove(animal);
        }

        public void SetSpawnCooldown(float cooldown)
        {
            _contextProvider.Get<IZooContext>().SpawnCooldown = Math.Max(0f, cooldown);
        }
    }
}
