using ZW.Data.Models.Types;

namespace ZW.Logic.Interfaces.Systems.Movements
{
    public interface IAnimalObstacleSystem
    {
        void HitObstacle(int uid, PlanarVector obstacleNormal);
    }
}
