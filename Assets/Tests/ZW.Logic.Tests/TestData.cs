using ZW.Data.Generated.Entities.Animals;
using ZW.Data.Generated.Entities.FoodChains;
using ZW.Data.Generated.Models.Animals;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Entities.FoodChains;
using ZW.Data.Interfaces.Entities.Movements;
using ZW.Data.Interfaces.Models.Animals;

namespace ZW.Logic.Tests
{
    public static class TestData
    {
        public static readonly IFoodChainEntity Prey = new FoodChainEntity { Id = "prey" };
        public static readonly IFoodChainEntity Predator = new FoodChainEntity { Id = "predator" };

        public static IPredationRuleEntity Rule(IFoodChainEntity predator, IFoodChainEntity prey)
        {
            return new PredationRuleEntity { Id = $"{predator.Id}-{prey.Id}", Predator = predator, Prey = prey };
        }

        public static IAnimalEntity Animal(string id, IFoodChainEntity foodChain = null, int strength = 1, float spawnWeight = 1f, IMovementEntity movement = null)
        {
            return new AnimalEntity { Id = id, FoodChain = foodChain, Movement = movement, Strength = strength, SpawnWeight = spawnWeight };
        }

        public static IAnimalModel Model(IAnimalEntity entity, float moveCooldown = 0f)
        {
            return new AnimalModel { Entity = entity, MoveCooldown = moveCooldown };
        }
    }
}
