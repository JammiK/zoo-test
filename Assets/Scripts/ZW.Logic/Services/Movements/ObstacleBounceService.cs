using System;
using ZW.Data.Models.Types;
using ZW.Logic.Extensions;
using ZW.Logic.Interfaces.Services.Movements;

namespace ZW.Logic.Services.Movements
{
    public class ObstacleBounceService : IObstacleBounceService
    {
        private const float MaxDeviation = 75f * (float)Math.PI / 180f;
        private const float MinNormalLength = 0.1f;

        public PlanarVector? GetDirectionAway(PlanarVector obstacleNormal, float roll)
        {
            if (obstacleNormal.Magnitude() < MinNormalLength)
            {
                return null;
            }

            var deviation = (Math.Clamp(roll, 0f, 1f) * 2f - 1f) * MaxDeviation;
            return obstacleNormal.Normalized().Rotate(deviation);
        }
    }
}
