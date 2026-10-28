using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Models.Types;

namespace ZW.Logic.Interfaces.Systems.FoodChains
{
    public interface IAnimalDeathSystem
    {
        void Eat(IAnimalModel eater, IAnimalModel victim, PlanarVector position);
    }
}
