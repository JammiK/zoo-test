using NUnit.Framework;
using ZW.Data.Generated.Entities.Levels;
using ZW.Data.Generated.Entities.Settings;
using ZW.Data.Models.Types;
using ZW.Logic.Interfaces.Services.Spawns;
using ZW.Logic.Services.Levels;
using ZW.Logic.Services.Spawns;
using ZW.Logic.Tests.Fakes;

namespace ZW.Logic.Tests.Spawns
{
    public class SpawnPointServiceTests
    {
        private LevelAreaService _area;

        [SetUp]
        public void SetUp()
        {
            var settings = new ZooSettingsEntity
            {
                Level = new LevelEntity { Size = new PlanarVector(20f, 10f) },
                ScreenPadding = 1f
            };
            _area = new LevelAreaService(new FakeSettingsRepository(settings), new FakeRandomService { Value = 0.5f });
        }

        [Test]
        public void NoValidators_AcceptsFirstPoint()
        {
            var service = new SpawnPointService(_area, new ISpawnPointValidator[0]);

            Assert.IsTrue(service.FindFreePoint(0.5f).HasValue);
        }

        [Test]
        public void RejectedPoints_AreRetried()
        {
            var validator = new FakeSpawnPointValidator { RejectFirst = 5 };
            var service = new SpawnPointService(_area, new ISpawnPointValidator[] { validator });

            Assert.IsTrue(service.FindFreePoint(0.5f).HasValue);
            Assert.AreEqual(6, validator.Calls);
        }

        [Test]
        public void NoFreePoint_ReturnsNull()
        {
            var validator = new FakeSpawnPointValidator { RejectFirst = int.MaxValue };
            var service = new SpawnPointService(_area, new ISpawnPointValidator[] { validator });

            Assert.IsNull(service.FindFreePoint(0.5f));
        }

        [Test]
        public void RadiusIsPassedToValidators()
        {
            var validator = new FakeSpawnPointValidator();
            var service = new SpawnPointService(_area, new ISpawnPointValidator[] { validator });

            service.FindFreePoint(1.25f);

            Assert.AreEqual(1.25f, validator.LastRadius);
        }

        private class FakeSpawnPointValidator : ISpawnPointValidator
        {
            public int RejectFirst { get; set; }
            public int Calls { get; private set; }
            public float LastRadius { get; private set; }

            public bool IsFree(PlanarVector point, float radius)
            {
                Calls++;
                LastRadius = radius;
                return Calls > RejectFirst;
            }
        }
    }
}
