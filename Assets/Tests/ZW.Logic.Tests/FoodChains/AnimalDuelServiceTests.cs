using NUnit.Framework;
using ZW.Logic.Services.FoodChains;
using static ZW.Logic.Tests.TestData;

namespace ZW.Logic.Tests.FoodChains
{
    public class AnimalDuelServiceTests
    {
        private readonly AnimalDuelService _service = new();

        [TestCase(0f)]
        [TestCase(0.99f)]
        public void StrongerWins_RegardlessOfRoll(float roll)
        {
            var weak = Model(Animal("weak", Predator, strength: 1));
            var strong = Model(Animal("strong", Predator, strength: 5));

            Assert.AreSame(strong, _service.GetWinner(weak, strong, roll));
            Assert.AreSame(strong, _service.GetWinner(strong, weak, roll));
        }

        [Test]
        public void EqualStrength_RollDecides()
        {
            var first = Model(Animal("first", Predator));
            var second = Model(Animal("second", Predator));

            Assert.AreSame(first, _service.GetWinner(first, second, 0.2f));
            Assert.AreSame(second, _service.GetWinner(first, second, 0.8f));
        }
    }
}
