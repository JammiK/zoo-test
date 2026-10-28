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
            var zoo = _contextProvider.Get<IZooContext>();
            zoo.Animals.Add(animal);
            zoo.AnimalsByUid[animal.Uid] = animal;
        }

        public void RemoveAnimal(IAnimalModel animal)
        {
            var zoo = _contextProvider.Get<IZooContext>();
            zoo.Animals.Remove(animal);
            zoo.AnimalsByUid.Remove(animal.Uid);
        }

        public int RestoreIndex()
        {
            var zoo = _contextProvider.Get<IZooContext>();
            var removed = zoo.Animals.RemoveAll(animal => animal.Entity == null);

            zoo.AnimalsByUid.Clear();
            foreach (var animal in zoo.Animals)
            {
                zoo.AnimalsByUid[animal.Uid] = animal;
            }

            return removed;
        }

        public void SetSpawnCooldown(float cooldown)
        {
            _contextProvider.Get<IZooContext>().SpawnCooldown = Math.Max(0f, cooldown);
        }
    }
}
