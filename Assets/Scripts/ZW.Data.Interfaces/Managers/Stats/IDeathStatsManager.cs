using ZW.Data.Interfaces.Entities.FoodChains;

namespace ZW.Data.Interfaces.Managers.Stats
{
    public interface IDeathStatsManager
    {
        void AddDeath(IFoodChainEntity foodChain);
    }
}
