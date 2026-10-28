using ZW.Data.Models.Types;

namespace ZW.Logic.Interfaces.Systems.FoodChains
{
    public interface IAnimalCollisionSystem
    {
        void Collide(int firstUid, int secondUid, PlanarVector contactPoint);
    }
}
