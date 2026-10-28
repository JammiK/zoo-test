using Core.JG.Presenters.Core.Interfaces.GameAssets;
using Cysharp.Threading.Tasks;
using JG.Data.Entities.Interfaces.Repositories.Common;
using UnityEngine;
using ZW.Data.Interfaces.Entities.Settings;
using ZW.Views.Interfaces.Actors.World;

namespace ZW.Presenters.Presenters.Levels
{
    public class LevelPresenter
    {
        public LevelPresenter(
            ISettingsRepository settingsRepository,
            IInstantiateSystem instantiateSystem,
            IWorldContainerView worldContainerView,
            ICameraView cameraView)
        {
            var level = settingsRepository.Get<IZooSettingsEntity>().Level;
            cameraView.Fit(level.Size.X, level.Size.Z);

            if (string.IsNullOrEmpty(level.Prefab))
            {
                Debug.LogError($"Level '{level.Id}' has no prefab");
                return;
            }

            instantiateSystem
                .Instantiate<Transform>(level.Prefab, worldContainerView.LevelContainer, worldContainerView.LifetimeToken)
                .Forget();
        }
    }
}
