using NUnit.Framework;
using ZW.Data.Generated.Entities.Levels;
using ZW.Data.Generated.Entities.Settings;
using ZW.Data.Models.Types;
using ZW.Logic.Extensions;
using ZW.Logic.Services.Levels;
using ZW.Logic.Services.Movements;
using ZW.Logic.Tests.Fakes;

namespace ZW.Logic.Tests.Levels
{
    public class AreaReturnServiceTests
    {
        private static readonly PlanarVector OutsideSouth = new(0f, -6f);
        private static readonly PlanarVector North = new(0f, 1f);

        private AreaReturnService _service;

        [SetUp]
        public void SetUp()
        {
            var settings = new ZooSettingsEntity
            {
                Level = new LevelEntity { Size = new PlanarVector(20f, 10f) },
                ScreenPadding = 1f
            };
            var area = new LevelAreaService(new FakeSettingsRepository(settings), new FakeRandomService());
            _service = new AreaReturnService(area, new ObstacleBounceService());
        }

        [Test]
        public void AlreadyHeadingInside_KeepsDirection()
        {
            var heading = new PlanarVector(0.6f, 0.8f);

            Assert.AreEqual(heading, _service.GetReturnDirection(OutsideSouth, heading, 0.1f));
        }

        [TestCase(0f)]
        [TestCase(0.5f)]
        [TestCase(1f)]
        public void HeadingOutside_TurnsBackInside(float roll)
        {
            var direction = _service.GetReturnDirection(OutsideSouth, new PlanarVector(0f, -1f), roll);

            Assert.Greater(direction.Dot(North), 0.2f);
        }
    }
}
