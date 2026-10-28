using System;
using JG.Data.Entities.Interfaces.Repositories.Common;
using UniRx;
using ZW.Data.Interfaces.Entities.FoodChains;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Interfaces.Providers.Stats;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Views.Interfaces.UI.Stats;

namespace ZW.Presenters.Presenters.Stats
{
    public class DeathStatsPresenter : IDisposable
    {
        private readonly IDeathCounterView[] _views;
        private readonly IDeathStatsProvider _deathStatsProvider;
        private readonly IDisposable _diedSubscription;

        public DeathStatsPresenter(
            IDeathCounterView[] views,
            IDeathStatsProvider deathStatsProvider,
            IEntityRepository<IFoodChainEntity> foodChainRepository,
            IAnimalDiedStream diedStream)
        {
            _views = views;
            _deathStatsProvider = deathStatsProvider;

            foreach (var view in _views)
            {
                view.SetTitle(foodChainRepository.Get(view.FoodChainId)?.Title ?? view.FoodChainId);
                view.SetCount(_deathStatsProvider.GetDeaths(view.FoodChainId));
            }

            _diedSubscription = diedStream.Subscribe(Refresh);
        }

        public void Dispose()
        {
            _diedSubscription.Dispose();
        }

        private void Refresh(IAnimalModel victim)
        {
            var foodChainId = victim.Entity.FoodChain.Id;
            foreach (var view in _views)
            {
                if (view.FoodChainId == foodChainId)
                {
                    view.SetCount(_deathStatsProvider.GetDeaths(foodChainId));
                }
            }
        }
    }
}
