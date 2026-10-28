using NUnit.Framework;
using ZW.Data.Interfaces.Entities.FoodChains;
using ZW.Logic.Models.FoodChains;
using ZW.Logic.Services.FoodChains;
using ZW.Logic.Tests.Fakes;
using static ZW.Logic.Tests.TestData;

namespace ZW.Logic.Tests.FoodChains
{
    public class PredationServiceTests
    {
        private PredationService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new PredationService(new FakeEntityRepository<IPredationRuleEntity>(
                Rule(Predator, Prey),
                Rule(Predator, Predator)));
        }

        [Test]
        public void PreyMeetsPrey_NobodyEats()
        {
            Assert.AreEqual(PredationOutcome.None, _service.Resolve(Prey, Prey));
        }

        [Test]
        public void PredatorMeetsPrey_PredatorEats()
        {
            Assert.AreEqual(PredationOutcome.FirstEatsSecond, _service.Resolve(Predator, Prey));
            Assert.AreEqual(PredationOutcome.SecondEatsFirst, _service.Resolve(Prey, Predator));
        }

        [Test]
        public void PredatorMeetsPredator_Duel()
        {
            Assert.AreEqual(PredationOutcome.Duel, _service.Resolve(Predator, Predator));
        }
    }
}
