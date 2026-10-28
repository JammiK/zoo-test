using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Models.Types;

namespace ZW.Logic.Models.FoodChains
{
    public readonly struct AnimalEaten
    {
        public readonly IAnimalModel Eater;
        public readonly IAnimalModel Victim;
        public readonly PlanarVector Position;

        public AnimalEaten(IAnimalModel eater, IAnimalModel victim, PlanarVector position)
        {
            Eater = eater;
            Victim = victim;
            Position = position;
        }
    }
}
