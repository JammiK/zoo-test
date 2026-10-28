using System.Linq;
using ZW.Data.Models.Types;
using ZW.Logic.Interfaces.Services.Levels;
using ZW.Logic.Interfaces.Services.Spawns;

namespace ZW.Logic.Services.Spawns
{
    public class SpawnPointService : ISpawnPointService
    {
        private const int MaxAttempts = 16;

        private readonly ILevelAreaService _levelAreaService;
        private readonly ISpawnPointValidator[] _validators;

        public SpawnPointService(ILevelAreaService levelAreaService, ISpawnPointValidator[] validators)
        {
            _levelAreaService = levelAreaService;
            _validators = validators;
        }

        public PlanarVector? FindFreePoint(float radius)
        {
            for (var attempt = 0; attempt < MaxAttempts; attempt++)
            {
                var point = _levelAreaService.RandomPoint();
                if (_validators.All(validator => validator.IsFree(point, radius)))
                {
                    return point;
                }
            }

            return null;
        }
    }
}
