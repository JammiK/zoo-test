using System;
using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Repositories.Common;
using UniRx;
using ZW.Data.Interfaces.Entities.Settings;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Logic.Models.FoodChains;
using ZW.Presenters.Interfaces.Providers.Animals;

namespace ZW.Presenters.Presenters.Animals
{
    public class TastyLabelPresenter : IDisposable
    {
        private const string TastyText = "Tasty!";

        private readonly ISettingsRepository _settingsRepository;
        private readonly IAnimalActorProvider _actorProvider;
        private readonly IDisposable _eatenSubscription;
        private readonly Dictionary<int, IDisposable> _hideTimers = new();

        public TastyLabelPresenter(
            IAnimalEatenStream eatenStream,
            IAnimalActorProvider actorProvider,
            ISettingsRepository settingsRepository)
        {
            _actorProvider = actorProvider;
            _settingsRepository = settingsRepository;
            _eatenSubscription = eatenStream.Subscribe(Show);
        }

        public void Dispose()
        {
            _eatenSubscription.Dispose();
            foreach (var timer in _hideTimers.Values)
            {
                timer.Dispose();
            }
        }

        private void Show(AnimalEaten eaten)
        {
            var uid = eaten.Eater.Uid;
            if (!_actorProvider.TryGet(uid, out var actor))
            {
                return;
            }

            actor.SetLabel(TastyText);
            actor.SetLabelVisible(true);

            if (_hideTimers.Remove(uid, out var previous))
            {
                previous.Dispose();
            }

            var duration = _settingsRepository.Get<IZooSettingsEntity>().TastyLabelDuration;
            _hideTimers[uid] = Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe(_ => Hide(uid));
        }

        private void Hide(int uid)
        {
            _hideTimers.Remove(uid);
            if (_actorProvider.TryGet(uid, out var actor))
            {
                actor.SetLabelVisible(false);
            }
        }
    }
}
