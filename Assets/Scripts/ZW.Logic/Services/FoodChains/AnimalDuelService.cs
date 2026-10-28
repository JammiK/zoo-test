using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Interfaces.Services.FoodChains;

namespace ZW.Logic.Services.FoodChains
{
    public class AnimalDuelService : IAnimalDuelService
    {
        public IAnimalModel GetWinner(IAnimalModel first, IAnimalModel second, float roll)
        {
            if (first.Entity.Strength != second.Entity.Strength)
            {
                return first.Entity.Strength > second.Entity.Strength ? first : second;
            }

            return roll < 0.5f ? first : second;
        }
    }
}
