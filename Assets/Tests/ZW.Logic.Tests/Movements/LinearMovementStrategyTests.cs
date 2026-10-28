using NUnit.Framework;
using ZW.Data.Generated.Entities.Movements;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Entities.Movements;
using ZW.Data.Models.Types;
using ZW.Logic.Models.Movements;
using ZW.Logic.Services.Movements;
using ZW.Logic.Tests.Fakes;
using static ZW.Logic.Tests.TestData;

namespace ZW.Logic.Tests.Movements
{
    public class LinearMovementStrategyTests
    {
        private IAnimalEntity _snake;
        private LinearMovementStrategy _strategy;

        [SetUp]
        public void SetUp()
        {
            _snake = Animal("snake", Predator);
            var config = new LinearMovementEntity { Id = "linear", Animals = new[] { _snake }, Speed = 3f };
            _strategy = new LinearMovementStrategy(new FakeEntityRepository<ILinearMovementEntity>(config));
        }

        [Test]
        public void MovesWithConstantSpeedEveryTick()
        {
            var animal = Model(_snake);
            animal.Direction = new PlanarVector(1f, 0f);

            var decision = _strategy.Decide(animal, new MovementContext(0.5f, true, default));

            Assert.AreEqual(new PlanarVector(3f, 0f), decision.Motion.Value.Velocity);
        }

        [Test]
        public void InsideArea_NeverChangesDirectionByItself()
        {
            var animal = Model(_snake);
            animal.Direction = new PlanarVector(0f, 1f);

            for (var tick = 0; tick < 100; tick++)
            {
                var decision = _strategy.Decide(animal, new MovementContext(0.5f, true, default));
                Assert.AreEqual(new PlanarVector(0f, 1f), decision.Direction);
            }
        }

        [Test]
        public void OutsideArea_HeadsBack()
        {
            var back = new PlanarVector(0f, -1f);

            var decision = _strategy.Decide(Model(_snake), new MovementContext(0.1f, false, back));

            Assert.AreEqual(back, decision.Direction);
            Assert.AreEqual(new PlanarVector(0f, -3f), decision.Motion.Value.Velocity);
        }
    }
}
