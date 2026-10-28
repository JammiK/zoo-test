using Core.JG.Presenters.Core.Interfaces.GameAssets;
using Cysharp.Threading.Tasks;
using JG.Data.Entities.Interfaces.Repositories.Common;
using UnityEngine;
using ZW.Data.Interfaces.Entities.Levels;
using ZW.Data.Interfaces.Entities.Settings;
using ZW.Logic.Interfaces.Systems.Zoo;
using ZW.Views.Interfaces.Actors.World;

namespace ZW.Presenters.Presenters.Levels
{
    public class LevelPresenter
    {
        private readonly IInstantiateSystem _instantiateSystem;
        private readonly IWorldContainerView _worldContainerView;
        private readonly IZooStartSystem _zooStartSystem;

        public LevelPresenter(
            ISettingsRepository settingsRepository,
            IInstantiateSystem instantiateSystem,
            IWorldContainerView worldContainerView,
            ICameraView cameraView,
            IZooStartSystem zooStartSystem)
        {
            _instantiateSystem = instantiateSystem;
            _worldContainerView = worldContainerView;
            _zooStartSystem = zooStartSystem;

            var level = settingsRepository.Get<IZooSettingsEntity>().Level;
            cameraView.Fit(level.Size.X, level.Size.Z);
            LoadAndStartAsync(level).Forget();
        }

        private async UniTaskVoid LoadAndStartAsync(ILevelEntity level)
        {
            if (string.IsNullOrEmpty(level.Prefab))
            {
                Debug.LogError($"Level '{level.Id}' has no prefab");
            }
            else if (await _instantiateSystem.Instantiate<Transform>(level.Prefab, _worldContainerView.LevelContainer, _worldContainerView.LifetimeToken) == null)
            {
                Debug.LogError($"Level '{level.Id}' failed to load");
            }

            _zooStartSystem.Start();
        }
    }
}
