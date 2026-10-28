using NUnit.Framework;
using ZW.Data.Generated.Entities.Levels;
using ZW.Data.Generated.Entities.Settings;
using ZW.Data.Models.Types;
using ZW.Logic.Services.Levels;
using ZW.Logic.Tests.Fakes;

namespace ZW.Logic.Tests.Levels
{
    public class LevelAreaServiceTests
    {
        private FakeRandomService _random;
        private LevelAreaService _service;

        [SetUp]
        public void SetUp()
        {
            _random = new FakeRandomService();
            var settings = new ZooSettingsEntity
            {
                Level = new LevelEntity { Size = new PlanarVector(20f, 10f) },
                ScreenPadding = 1f
            };
            _service = new LevelAreaService(new FakeSettingsRepository(settings), _random);
        }

        [TestCase(0f, 0f, true)]
        [TestCase(8.9f, -3.9f, true)]
        [TestCase(9.5f, 0f, false)]
        [TestCase(0f, 4.5f, false)]
        public void IsInside_RespectsSizeAndPadding(float x, float z, bool expected)
        {
            Assert.AreEqual(expected, _service.IsInside(new PlanarVector(x, z)));
        }

        [Test]
        public void DirectionToCenter_IsNormalized()
        {
            var direction = _service.DirectionToCenter(new PlanarVector(5f, 0f));

            Assert.AreEqual(-1f, direction.X, 1e-5f);
            Assert.AreEqual(0f, direction.Z, 1e-5f);
        }

        [TestCase(0f)]
        [TestCase(1f)]
        public void RandomPoint_StaysInside(float roll)
        {
            _random.Value = roll;

            Assert.IsTrue(_service.IsInside(_service.RandomPoint()));
        }
    }
}
