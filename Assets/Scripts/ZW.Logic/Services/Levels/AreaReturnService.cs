using ZW.Data.Models.Types;
using ZW.Logic.Extensions;
using ZW.Logic.Interfaces.Services.Levels;
using ZW.Logic.Interfaces.Services.Movements;

namespace ZW.Logic.Services.Levels
{
    public class AreaReturnService : IAreaReturnService
    {
        private readonly ILevelAreaService _levelAreaService;
        private readonly IObstacleBounceService _bounceService;

        public AreaReturnService(ILevelAreaService levelAreaService, IObstacleBounceService bounceService)
        {
            _levelAreaService = levelAreaService;
            _bounceService = bounceService;
        }

        public PlanarVector GetReturnDirection(PlanarVector position, PlanarVector currentDirection, float roll)
        {
            var inward = _levelAreaService.InwardNormal(position);
            if (currentDirection.Dot(inward) > 0f)
            {
                return currentDirection;
            }

            return _bounceService.GetDirectionAway(inward, roll) ?? inward;
        }
    }
}
