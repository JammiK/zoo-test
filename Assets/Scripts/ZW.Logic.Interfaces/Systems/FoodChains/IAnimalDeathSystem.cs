using ZW.Data.Interfaces.Models.Animals;

namespace ZW.Logic.Interfaces.Systems.FoodChains
{
    public interface IAnimalDeathSystem
    {
        void Eat(IAnimalModel eater, IAnimalModel victim);
    }
}
