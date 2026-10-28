using ZW.Data.Interfaces.Entities.FoodChains;
using ZW.Logic.Models.FoodChains;

namespace ZW.Logic.Interfaces.Services.FoodChains
{
    public interface IPredationService
    {
        PredationOutcome Resolve(IFoodChainEntity first, IFoodChainEntity second);
    }
}
