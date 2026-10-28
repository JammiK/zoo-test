using ZW.Data.Models.Types;

namespace ZW.Logic.Interfaces.Services.Movements
{
    public interface IObstacleBounceService
    {
        PlanarVector? GetDirectionAway(PlanarVector obstacleNormal, float roll);
    }
}
