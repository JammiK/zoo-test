using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Repositories.Common;
using ZW.Data.Interfaces.Entities.FoodChains;
using ZW.Logic.Interfaces.Services.FoodChains;
using ZW.Logic.Models.FoodChains;

namespace ZW.Logic.Services.FoodChains
{
    public class PredationService : IPredationService
    {
        private readonly HashSet<(string Predator, string Prey)> _rules = new();

        public PredationService(IEntityRepository<IPredationRuleEntity> ruleRepository)
        {
            foreach (var rule in ruleRepository.GetAll())
            {
                if (rule.Predator != null && rule.Prey != null)
                {
                    _rules.Add((rule.Predator.Id, rule.Prey.Id));
                }
            }
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
            return predator != null && prey != null && _rules.Contains((predator.Id, prey.Id));
        }
    }
}
