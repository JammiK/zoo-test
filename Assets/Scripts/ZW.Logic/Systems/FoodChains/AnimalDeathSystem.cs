using ZW.Data.Interfaces.Managers.Animals;
using ZW.Data.Interfaces.Managers.Stats;
using ZW.Data.Interfaces.Managers.Zoo;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Logic.Interfaces.Systems.FoodChains;
using ZW.Logic.Models.FoodChains;

namespace ZW.Logic.Systems.FoodChains
{
    public class AnimalDeathSystem : IAnimalDeathSystem
    {
        private readonly IAnimalManager _animalManager;
        private readonly IDeathStatsManager _deathStatsManager;
        private readonly IZooManager _zooManager;
        private readonly IAnimalEatenStream _eatenStream;
        private readonly IAnimalDiedStream _diedStream;

        public AnimalDeathSystem(
            IAnimalManager animalManager,
            IDeathStatsManager deathStatsManager,
            IZooManager zooManager,
            IAnimalEatenStream eatenStream,
            IAnimalDiedStream diedStream)
        {
            _animalManager = animalManager;
            _deathStatsManager = deathStatsManager;
            _zooManager = zooManager;
            _eatenStream = eatenStream;
            _diedStream = diedStream;
        }

        public void Eat(IAnimalModel eater, IAnimalModel victim)
        {
            _animalManager.Kill(victim);
            _deathStatsManager.AddDeath(victim.Entity.FoodChain);
            _zooManager.RemoveAnimal(victim);

            _eatenStream.OnNext(new AnimalEaten(eater, victim));
            _diedStream.OnNext(victim);
        }
    }
}
