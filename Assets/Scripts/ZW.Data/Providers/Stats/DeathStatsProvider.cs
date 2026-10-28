using JG.Data.Entities.Interfaces.Providers.Contexts;
using ZW.Data.Interfaces.Contexts;
using ZW.Data.Interfaces.Providers.Stats;

namespace ZW.Data.Providers.Stats
{
    public class DeathStatsProvider : IDeathStatsProvider
    {
        private readonly IContextProvider _contextProvider;

        public DeathStatsProvider(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public int GetDeaths(string foodChainId)
        {
            return _contextProvider.Get<IDeathStatsContext>().DeathsByFoodChain.TryGetValue(foodChainId, out var count) ? count : 0;
        }
    }
}
