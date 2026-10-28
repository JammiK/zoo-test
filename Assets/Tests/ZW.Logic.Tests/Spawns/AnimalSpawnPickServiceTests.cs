using NUnit.Framework;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Logic.Services.Spawns;
using static ZW.Logic.Tests.TestData;

namespace ZW.Logic.Tests.Spawns
{
    public class AnimalSpawnPickServiceTests
    {
        private readonly AnimalSpawnPickService _service = new();

        [Test]
        public void NoCandidates_ReturnsNull()
        {
            Assert.IsNull(_service.Pick(new IAnimalEntity[0], 0.5f));
        }

        [Test]
        public void ZeroWeight_NeverPicked()
        {
            var hidden = Animal("hidden", spawnWeight: 0f);
            var visible = Animal("visible", spawnWeight: 1f);

            Assert.AreSame(visible, _service.Pick(new[] { hidden, visible }, 0f));
            Assert.AreSame(visible, _service.Pick(new[] { hidden, visible }, 0.99f));
        }

        [TestCase(0f, "rare")]
        [TestCase(0.2f, "rare")]
        [TestCase(0.3f, "common")]
        [TestCase(1f, "common")]
        public void PickIsProportionalToWeight(float roll, string expectedId)
        {
            var rare = Animal("rare", spawnWeight: 1f);
            var common = Animal("common", spawnWeight: 3f);

            Assert.AreEqual(expectedId, _service.Pick(new[] { rare, common }, roll).Id);
        }
    }
}
