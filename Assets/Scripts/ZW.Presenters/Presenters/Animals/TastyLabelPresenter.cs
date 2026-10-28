using System;
using System.Collections.Generic;
using Core.JG.Logic.Core.Interfaces.Systems.Core;
using Cysharp.Threading.Tasks;
using JG.Data.Entities.Interfaces.Repositories.Common;
using UniRx;
using ZW.Data.Interfaces.Entities.Settings;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Logic.Models.FoodChains;
using ZW.Presenters.Interfaces.Pools;
using ZW.Presenters.Interfaces.Providers.Animals;
using ZW.Views.Interfaces.Actors.Effects;
using ZW.Views.Interfaces.Actors.World;

namespace ZW.Presenters.Presenters.Animals
{
    public class TastyLabelPresenter : IDisposable
    {
        private const string TastyText = "Tasty!";

        private readonly ISettingsRepository _settingsRepository;
        private readonly IAnimalActorProvider _actorProvider;
        private readonly IViewPoolService _viewPoolService;
        private readonly IWorldContainerView _worldContainerView;
        private readonly IUpdateSystem _updateSystem;
        private readonly IDisposable _eatenSubscription;
        private readonly IDisposable _diedSubscription;
        private readonly Dictionary<int, ITastyLabelView> _labels = new();
        private readonly Dictionary<int, IDisposable> _hideTimers = new();
        private readonly HashSet<int> _pending = new();
        private readonly List<int> _followBuffer = new();

        public TastyLabelPresenter(
            IAnimalEatenStream eatenStream,
            IAnimalDiedStream diedStream,
            IAnimalActorProvider actorProvider,
            ISettingsRepository settingsRepository,
            IViewPoolService viewPoolService,
            IWorldContainerView worldContainerView,
            IUpdateSystem updateSystem)
        {
            _actorProvider = actorProvider;
            _settingsRepository = settingsRepository;
            _viewPoolService = viewPoolService;
            _worldContainerView = worldContainerView;
            _updateSystem = updateSystem;

            _eatenSubscription = eatenStream.Subscribe(eaten => ShowAsync(eaten).Forget());
            _diedSubscription = diedStream.Subscribe(OnDied);
            _updateSystem.Add(FollowEaters);
        }

        private IZooSettingsEntity Settings => _settingsRepository.Get<IZooSettingsEntity>();

        public void Dispose()
        {
            _eatenSubscription.Dispose();
            _diedSubscription.Dispose();
            _updateSystem.Remove(FollowEaters);
            foreach (var timer in _hideTimers.Values)
            {
                timer.Dispose();
            }
        }

        private async UniTaskVoid ShowAsync(AnimalEaten eaten)
        {
            var uid = eaten.Eater.Uid;
            if (_pending.Contains(uid))
            {
                return;
            }

            if (!_labels.TryGetValue(uid, out var label))
            {
                _pending.Add(uid);
                label = await _viewPoolService.Get<ITastyLabelView>(
                    Settings.TastyLabelPrefab,
                    _worldContainerView.EffectsContainer,
                    _worldContainerView.LifetimeToken);
                _pending.Remove(uid);

                if (label == null)
                {
                    return;
                }

                if (!_actorProvider.TryGet(uid, out _))
                {
                    _viewPoolService.Release(Settings.TastyLabelPrefab, label);
                    return;
                }

                label.SetText(TastyText);
                _labels[uid] = label;
            }

            FollowEater(uid, label);
            RestartHideTimer(uid);
        }

        private void RestartHideTimer(int uid)
        {
            if (_hideTimers.Remove(uid, out var previous))
            {
                previous.Dispose();
            }

            _hideTimers[uid] = Observable.Timer(TimeSpan.FromSeconds(Settings.TastyLabelDuration)).Subscribe(_ => Hide(uid));
        }

        private void OnDied(IAnimalModel victim)
        {
            Hide(victim.Uid);
        }

        private void Hide(int uid)
        {
            if (_hideTimers.Remove(uid, out var timer))
            {
                timer.Dispose();
            }

            if (_labels.Remove(uid, out var label))
            {
                _viewPoolService.Release(Settings.TastyLabelPrefab, label);
            }
        }

        private void FollowEaters()
        {
            _followBuffer.Clear();
            _followBuffer.AddRange(_labels.Keys);
            foreach (var uid in _followBuffer)
            {
                if (_labels.TryGetValue(uid, out var label))
                {
                    FollowEater(uid, label);
                }
            }
        }

        private void FollowEater(int uid, ITastyLabelView label)
        {
            if (_actorProvider.TryGet(uid, out var actor))
            {
                label.SetAnchor(actor.Position);
            }
            else
            {
                Hide(uid);
            }
        }
    }
}
