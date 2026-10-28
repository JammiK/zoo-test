using System;
using Core.JG.Data.Interfaces.Services;
using JG.Data.Entities.Interfaces.Repositories.Common;
using ZW.Data.Interfaces.Entities.Settings;
using ZW.Data.Models.Types;
using ZW.Logic.Extensions;
using ZW.Logic.Interfaces.Services.Levels;

namespace ZW.Logic.Services.Levels
{
    public class LevelAreaService : ILevelAreaService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IRandomService _randomService;

        public LevelAreaService(ISettingsRepository settingsRepository, IRandomService randomService)
        {
            _settingsRepository = settingsRepository;
            _randomService = randomService;
        }

        public bool IsInside(PlanarVector point)
        {
            var halfSize = GetHalfSize();
            return Math.Abs(point.X) <= halfSize.X && Math.Abs(point.Z) <= halfSize.Z;
        }

        public PlanarVector InwardNormal(PlanarVector point)
        {
            var halfSize = GetHalfSize();
            return new PlanarVector(InwardAxis(point.X, halfSize.X), InwardAxis(point.Z, halfSize.Z)).Normalized();
        }

        public PlanarVector RandomPoint()
        {
            var halfSize = GetHalfSize();
            return new PlanarVector(
                _randomService.Range(-halfSize.X, halfSize.X),
                _randomService.Range(-halfSize.Z, halfSize.Z));
        }

        private static float InwardAxis(float value, float halfSize)
        {
            if (value > halfSize)
            {
                return -1f;
            }

            return value < -halfSize ? 1f : 0f;
        }

        private PlanarVector GetHalfSize()
        {
            var settings = _settingsRepository.Get<IZooSettingsEntity>();
            var size = settings.Level.Size;
            return new PlanarVector(
                Math.Max(0f, size.X * 0.5f - settings.ScreenPadding),
                Math.Max(0f, size.Z * 0.5f - settings.ScreenPadding));
        }
    }
}
