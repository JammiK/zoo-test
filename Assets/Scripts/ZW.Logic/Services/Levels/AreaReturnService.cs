using System;
using ZW.Data.Models.Types;
using ZW.Logic.Extensions;
using ZW.Logic.Interfaces.Services.Levels;

namespace ZW.Logic.Services.Levels
{
    public class AreaReturnService : IAreaReturnService
    {
        private const float MaxDeviation = 30f * (float)Math.PI / 180f;
        private static readonly float KeepCourseMinDot = (float)Math.Cos(60f * Math.PI / 180f);

        private readonly ILevelAreaService _levelAreaService;

        public AreaReturnService(ILevelAreaService levelAreaService)
        {
            _levelAreaService = levelAreaService;
        }

        public PlanarVector GetReturnDirection(PlanarVector position, PlanarVector currentDirection, float roll)
        {
            var inward = _levelAreaService.InwardNormal(position);
            if (currentDirection.Normalized().Dot(inward) >= KeepCourseMinDot)
            {
                return currentDirection;
            }

            var deviation = (Math.Clamp(roll, 0f, 1f) * 2f - 1f) * MaxDeviation;
            return inward.Rotate(deviation);
        }
    }
}
