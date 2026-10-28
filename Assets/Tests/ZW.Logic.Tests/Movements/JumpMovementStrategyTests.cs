using System;
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
    public class JumpMovementStrategyTests
    {
        private const float Gravity = 10f;

        private IAnimalEntity _frog;
        private FakeRandomDirectionService _direction;
        private JumpMovementStrategy _strategy;

        [SetUp]
        public void SetUp()
        {
            _frog = Animal("frog", Prey);
            _direction = new FakeRandomDirectionService();
            var config = new JumpMovementEntity { Id = "jump", Animals = new[] { _frog }, Distance = 4f, Height = 1.25f, Interval = 2f };
            _strategy = new JumpMovementStrategy(
                new FakeEntityRepository<IJumpMovementEntity>(config),
                _direction,
                new FakeEnvironmentService { Gravity = Gravity });
        }

        [Test]
        public void CanMove_OnlyLinkedAnimals()
        {
            Assert.IsTrue(_strategy.CanMove(_frog));
            Assert.IsFalse(_strategy.CanMove(Animal("snake")));
        }

        [Test]
        public void WhileCooldown_DoesNotJump()
        {
            var decision = _strategy.Decide(Model(_frog, moveCooldown: 1f), Inside(0.25f));

            Assert.IsFalse(decision.Motion.HasValue);
            Assert.AreEqual(0.75f, decision.Cooldown, 1e-5f);
        }

        [Test]
        public void CooldownExpired_JumpsForConfiguredDistance()
        {
            _direction.Direction = new PlanarVector(0f, 1f);

            var decision = _strategy.Decide(Model(_frog), Inside(0.1f));

            var vertical = (float)Math.Sqrt(2f * Gravity * 1.25f);
            var airTime = 2f * vertical / Gravity;
            Assert.IsTrue(decision.Motion.HasValue);
            Assert.AreEqual(vertical, decision.Motion.Value.VerticalVelocity, 1e-4f);
            Assert.AreEqual(4f, decision.Motion.Value.Velocity.Z * airTime, 1e-4f);
            Assert.AreEqual(2f, decision.Cooldown, 1e-5f);
        }

        [Test]
        public void OutsideArea_JumpsBack()
        {
            var back = new PlanarVector(-1f, 0f);

            var decision = _strategy.Decide(Model(_frog), new MovementContext(0.1f, false, back));

            Assert.AreEqual(back, decision.Direction);
            Assert.Less(decision.Motion.Value.Velocity.X, 0f);
        }

        private static MovementContext Inside(float deltaTime)
        {
            return new MovementContext(deltaTime, true, default);
        }
    }
}
