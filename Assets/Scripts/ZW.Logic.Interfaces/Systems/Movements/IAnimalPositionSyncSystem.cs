using ZW.Data.Models.Types;

namespace ZW.Logic.Interfaces.Systems.Movements
{
    public interface IAnimalPositionSyncSystem
    {
        void Sync(int uid, PlanarVector position);
    }
}
