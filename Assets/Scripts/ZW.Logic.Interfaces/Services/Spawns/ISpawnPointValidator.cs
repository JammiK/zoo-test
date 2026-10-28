using ZW.Data.Models.Types;

namespace ZW.Logic.Interfaces.Services.Spawns
{
    public interface ISpawnPointValidator
    {
        bool IsFree(PlanarVector point, float radius);
    }
}
