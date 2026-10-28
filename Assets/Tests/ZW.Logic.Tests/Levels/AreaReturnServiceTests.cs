using System;
using NUnit.Framework;
using ZW.Data.Generated.Entities.Levels;
using ZW.Data.Generated.Entities.Settings;
using ZW.Data.Models.Types;
using ZW.Logic.Extensions;
using ZW.Logic.Services.Levels;
using ZW.Logic.Tests.Fakes;

namespace ZW.Logic.Tests.Levels
{
    public class AreaReturnServiceTests
    {
        private static readonly PlanarVector OutsideSouth = new(0f, -6f);
        private static readonly PlanarVector North = new(0f, 1f);
        private static readonly float Cos30 = (float)Math.Cos(30f * Math.PI / 180f);

        private AreaReturnService _service;

        [SetUp]
        public void SetUp()
        {
            var settings = new ZooSettingsEntity
            {
                Level = new LevelEntity { Size = new PlanarVector(20f, 10f) },
                ScreenPadding = 1f
            };
            _service = new AreaReturnService(new LevelAreaService(new FakeSettingsRepository(settings), new FakeRandomService()));
        }

        [Test]
        public void SteeplyHeadingInside_KeepsDirection()
        {
            var heading = new PlanarVector(0.6f, 0.8f);

            Assert.AreEqual(heading, _service.GetReturnDirection(OutsideSouth, heading, 0.1f));
        }

        [TestCase(0f)]
        [TestCase(0.5f)]
        [TestCase(1f)]
        public void HeadingOutside_TurnsSteeplyBackInside(float roll)
        {
            var direction = _service.GetReturnDirection(OutsideSouth, new PlanarVector(0f, -1f), roll);

            Assert.GreaterOrEqual(direction.Dot(North), Cos30 - 1e-4f);
        }

        [TestCase(0f)]
        [TestCase(1f)]
        public void ShallowInwardCourse_IsReplacedWithSteepOne(float roll)
        {
            var shallow = new PlanarVector(-0.96f, 0.28f);

            var direction = _service.GetReturnDirection(OutsideSouth, shallow, roll);

            Assert.GreaterOrEqual(direction.Dot(North), Cos30 - 1e-4f);
        }

        [Test]
        public void RepeatedJumps_ReturnInsideWithinTwoSteps()
        {
            var position = new PlanarVector(0f, -5.8f);
            var direction = new PlanarVector(0f, -1f);

            for (var jump = 0; jump < 2; jump++)
            {
                direction = _service.GetReturnDirection(position, direction, 0f);
                position = position.Add(direction.Scale(2f));
            }

            Assert.GreaterOrEqual(position.Z, -4f);
        }
    }
}
