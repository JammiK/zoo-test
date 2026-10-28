using System;
using NUnit.Framework;
using ZW.Data.Models.Types;
using ZW.Logic.Extensions;
using ZW.Logic.Services.Movements;

namespace ZW.Logic.Tests.Movements
{
    public class ObstacleBounceServiceTests
    {
        private readonly ObstacleBounceService _service = new();

        [Test]
        public void FloorContact_IsIgnored()
        {
            Assert.IsNull(_service.GetDirectionAway(default, 0.3f));
        }

        [Test]
        public void MiddleRoll_GoesStraightAlongNormal()
        {
            var direction = _service.GetDirectionAway(new PlanarVector(0f, -2f), 0.5f).Value;

            Assert.AreEqual(0f, direction.X, 1e-5f);
            Assert.AreEqual(-1f, direction.Z, 1e-5f);
        }

        [TestCase(0f)]
        [TestCase(0.25f)]
        [TestCase(0.75f)]
        [TestCase(1f)]
        public void AnyRoll_PointsAwayFromWallAndIsNormalized(float roll)
        {
            var normal = new PlanarVector(1f, 0f);

            var direction = _service.GetDirectionAway(normal, roll).Value;

            Assert.Greater(direction.Dot(normal), (float)Math.Cos(80f * Math.PI / 180f));
            Assert.AreEqual(1f, direction.Magnitude(), 1e-5f);
        }

        [Test]
        public void DifferentRolls_GiveDifferentDirections()
        {
            var normal = new PlanarVector(1f, 0f);

            Assert.AreNotEqual(_service.GetDirectionAway(normal, 0.1f), _service.GetDirectionAway(normal, 0.9f));
        }
    }
}
