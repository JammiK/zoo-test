using JG.Data.Entities.Interfaces.Repositories.Common;
using ZW.Data.Interfaces.Entities.FoodChains;
using ZW.Logic.Interfaces.Services.FoodChains;
using ZW.Logic.Models.FoodChains;

namespace ZW.Logic.Services.FoodChains
{
    public class PredationService : IPredationService
    {
        private readonly IEntityRepository<IPredationRuleEntity> _ruleRepository;

        public PredationService(IEntityRepository<IPredationRuleEntity> ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        public PredationOutcome Resolve(IFoodChainEntity first, IFoodChainEntity second)
        {
            var firstEats = CanEat(first, second);
            var secondEats = CanEat(second, first);

            if (firstEats && secondEats)
            {
                return PredationOutcome.Duel;
            }

            if (firstEats)
            {
                return PredationOutcome.FirstEatsSecond;
            }

            return secondEats ? PredationOutcome.SecondEatsFirst : PredationOutcome.None;
        }

        private bool CanEat(IFoodChainEntity predator, IFoodChainEntity prey)
        {
            return _ruleRepository.GetFirst(rule => rule.Predator?.Id == predator.Id && rule.Prey?.Id == prey.Id) != null;
        }
    }
}
