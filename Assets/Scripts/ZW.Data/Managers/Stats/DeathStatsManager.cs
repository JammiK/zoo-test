using JG.Data.Entities.Interfaces.Providers.Contexts;
using ZW.Data.Interfaces.Contexts;
using ZW.Data.Interfaces.Entities.FoodChains;
using ZW.Data.Interfaces.Managers.Stats;

namespace ZW.Data.Managers.Stats
{
    public class DeathStatsManager : IDeathStatsManager
    {
        private readonly IContextProvider _contextProvider;

        public DeathStatsManager(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public void AddDeath(IFoodChainEntity foodChain)
        {
            var deaths = _contextProvider.Get<IDeathStatsContext>().DeathsByFoodChain;
            deaths.TryGetValue(foodChain.Id, out var count);
            deaths[foodChain.Id] = count + 1;
        }
    }
}
