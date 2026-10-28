using ZW.Data.Models.Types;

namespace ZW.Logic.Interfaces.Services.Spawns
{
    public interface ISpawnPointService
    {
        PlanarVector? FindFreePoint(float radius);
    }
}
