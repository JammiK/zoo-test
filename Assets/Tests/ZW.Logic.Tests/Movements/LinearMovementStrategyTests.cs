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
        private FakeRandomDirectionService _direction;
        private LinearMovementStrategy _strategy;

        [SetUp]
        public void SetUp()
        {
            _snake = Animal("snake", Predator);
            _direction = new FakeRandomDirectionService { Direction = new PlanarVector(0f, 1f) };
            var config = new LinearMovementEntity { Id = "linear", Animals = new[] { _snake }, Speed = 3f, TurnInterval = 5f };
            _strategy = new LinearMovementStrategy(new FakeEntityRepository<ILinearMovementEntity>(config), _direction);
        }

        [Test]
        public void MovesWithConstantSpeedEveryTick()
        {
            var animal = Model(_snake, moveCooldown: 2f);
            animal.Direction = new PlanarVector(1f, 0f);

            var decision = _strategy.Decide(animal, new MovementContext(0.5f, true, default));

            Assert.AreEqual(new PlanarVector(3f, 0f), decision.Motion.Value.Velocity);
            Assert.AreEqual(1.5f, decision.Cooldown, 1e-5f);
        }

        [Test]
        public void TurnIntervalExpired_PicksNewDirection()
        {
            var decision = _strategy.Decide(Model(_snake), new MovementContext(0.1f, true, default));

            Assert.AreEqual(new PlanarVector(0f, 1f), decision.Direction);
            Assert.AreEqual(5f, decision.Cooldown, 1e-5f);
        }

        [Test]
        public void OutsideArea_HeadsBack()
        {
            var back = new PlanarVector(0f, -1f);

            var decision = _strategy.Decide(Model(_snake, moveCooldown: 2f), new MovementContext(0.1f, false, back));

            Assert.AreEqual(back, decision.Direction);
            Assert.AreEqual(new PlanarVector(0f, -3f), decision.Motion.Value.Velocity);
        }
    }
}
