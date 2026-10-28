using ZW.Data.Interfaces.Models.Animals;

namespace ZW.Logic.Interfaces.Services.FoodChains
{
    public interface IAnimalDuelService
    {
        IAnimalModel GetWinner(IAnimalModel first, IAnimalModel second, float roll);
    }
}
