using JG.Data.Entities.Interfaces.Attributes;
using JG.Data.Entities.Interfaces.Entities.Common;
using ZW.Data.Interfaces.Entities.FoodChains;
using ZW.Data.Interfaces.Entities.Movements;

namespace ZW.Data.Interfaces.Entities.Animals
{
    public interface IAnimalEntity : IEntity
    {
        [PrefabField] string Prefab { get; }
        IFoodChainEntity FoodChain { get; }
        IMovementEntity Movement { get; }
        int Strength { get; }
        [DefaultValueSnippet("1f")] float SpawnWeight { get; }
        [DefaultValueSnippet("0.5f")] float SpawnRadius { get; }
    }
}
