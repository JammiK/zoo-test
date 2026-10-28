using System;
using System.Collections.Generic;
using Core.JG.Logic.Core.Interfaces.Systems.Core;
using Cysharp.Threading.Tasks;
using JG.Data.Entities.Interfaces.Repositories.Common;
using UniRx;
using UnityEngine;
using ZW.Data.Interfaces.Entities.Settings;
using ZW.Data.Models.Types;
using ZW.Logic.Interfaces.Services.Environment;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Presenters.Extensions;
using ZW.Presenters.Interfaces.Pools;
using ZW.Views.Interfaces.Actors.Effects;
using ZW.Views.Interfaces.Actors.World;

namespace ZW.Presenters.Presenters.Animals
{
    public class TastyLabelPresenter : IDisposable
    {
        private const string TastyText = "Tasty!";
        private const float MinDuration = 0.01f;

        // The camera looks straight down, so world +Z is "up" on screen.
        private static readonly Vector3 ScreenUp = Vector3.forward;

        private readonly ISettingsRepository _settingsRepository;
        private readonly IViewPoolService _viewPoolService;
        private readonly IWorldContainerView _worldContainerView;
        private readonly IUpdateSystem _updateSystem;
        private readonly IEnvironmentService _environmentService;
        private readonly IDisposable _eatenSubscription;
        private readonly List<(ITastyLabelView View, Vector3 Origin, float Elapsed)> _labels = new();

        public TastyLabelPresenter(
            IAnimalEatenStream eatenStream,
            ISettingsRepository settingsRepository,
            IViewPoolService viewPoolService,
            IWorldContainerView worldContainerView,
            IUpdateSystem updateSystem,
            IEnvironmentService environmentService)
        {
            _settingsRepository = settingsRepository;
            _viewPoolService = viewPoolService;
            _worldContainerView = worldContainerView;
            _updateSystem = updateSystem;
            _environmentService = environmentService;

            _eatenSubscription = eatenStream.Subscribe(eaten => ShowAsync(eaten.Position).Forget());
            _updateSystem.Add(Animate);
        }

        private IZooSettingsEntity Settings => _settingsRepository.Get<IZooSettingsEntity>();

        public void Dispose()
        {
            _eatenSubscription.Dispose();
            _updateSystem.Remove(Animate);
            _labels.Clear();
        }

        private async UniTaskVoid ShowAsync(PlanarVector position)
        {
            var label = await _viewPoolService.Get<ITastyLabelView>(
                Settings.TastyLabelPrefab,
                _worldContainerView.EffectsContainer,
                _worldContainerView.LifetimeToken);
            if (label == null)
            {
                return;
            }

            var origin = position.ToWorld();
            label.SetText(TastyText);
            Apply(label, origin, 0f);
            _labels.Add((label, origin, 0f));
        }

        private void Animate()
        {
            var duration = Mathf.Max(Settings.TastyLabelDuration, MinDuration);
            var deltaTime = _environmentService.DeltaTime;
            for (var i = _labels.Count - 1; i >= 0; i--)
            {
                var (view, origin, elapsed) = _labels[i];
                elapsed += deltaTime;
                if (elapsed >= duration)
                {
                    _labels.RemoveAt(i);
                    _viewPoolService.Release(Settings.TastyLabelPrefab, view);
                    continue;
                }

                _labels[i] = (view, origin, elapsed);
                Apply(view, origin, elapsed / duration);
            }
        }

        private void Apply(ITastyLabelView view, Vector3 origin, float progress)
        {
            var rise = 1f - (1f - progress) * (1f - progress);
            view.SetAnchor(origin + ScreenUp * (Settings.TastyLabelRiseDistance * rise));
            view.SetAlpha(1f - progress * progress);
        }
    }
}
