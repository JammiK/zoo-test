using Core.JG.Data.Interfaces.Services;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Interfaces.Providers.Animals;
using ZW.Data.Interfaces.Types.Animals;
using ZW.Data.Models.Types;
using ZW.Logic.Interfaces.Services.FoodChains;
using ZW.Logic.Interfaces.Systems.FoodChains;
using ZW.Logic.Models.FoodChains;

namespace ZW.Logic.Systems.FoodChains
{
    public class AnimalCollisionSystem : IAnimalCollisionSystem
    {
        private readonly IAnimalProvider _animalProvider;
        private readonly IPredationService _predationService;
        private readonly IAnimalDuelService _duelService;
        private readonly IRandomService _randomService;
        private readonly IAnimalDeathSystem _deathSystem;

        public AnimalCollisionSystem(
            IAnimalProvider animalProvider,
            IPredationService predationService,
            IAnimalDuelService duelService,
            IRandomService randomService,
            IAnimalDeathSystem deathSystem)
        {
            _animalProvider = animalProvider;
            _predationService = predationService;
            _duelService = duelService;
            _randomService = randomService;
            _deathSystem = deathSystem;
        }

        public void Collide(int firstUid, int secondUid, PlanarVector contactPoint)
        {
            var first = _animalProvider.Get(firstUid);
            var second = _animalProvider.Get(secondUid);
            if (!IsAlive(first) || !IsAlive(second))
            {
                return;
            }

            switch (_predationService.Resolve(first.Entity.FoodChain, second.Entity.FoodChain))
            {
                case PredationOutcome.FirstEatsSecond:
                    _deathSystem.Eat(first, second, contactPoint);
                    break;
                case PredationOutcome.SecondEatsFirst:
                    _deathSystem.Eat(second, first, contactPoint);
                    break;
                case PredationOutcome.Duel:
                    var winner = _duelService.GetWinner(first, second, _randomService.NextFloat());
                    _deathSystem.Eat(winner, winner == first ? second : first, contactPoint);
                    break;
            }
        }

        private static bool IsAlive(IAnimalModel animal)
        {
            return animal != null && animal.State == AnimalState.Alive;
        }
    }
}
