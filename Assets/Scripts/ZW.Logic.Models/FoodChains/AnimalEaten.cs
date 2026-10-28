using ZW.Data.Interfaces.Models.Animals;

namespace ZW.Logic.Models.FoodChains
{
    public readonly struct AnimalEaten
    {
        public readonly IAnimalModel Eater;
        public readonly IAnimalModel Victim;

        public AnimalEaten(IAnimalModel eater, IAnimalModel victim)
        {
            Eater = eater;
            Victim = victim;
        }
    }
}
