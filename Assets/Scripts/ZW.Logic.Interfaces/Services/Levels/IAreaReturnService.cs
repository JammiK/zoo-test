using ZW.Data.Models.Types;

namespace ZW.Logic.Interfaces.Services.Levels
{
    public interface IAreaReturnService
    {
        PlanarVector GetReturnDirection(PlanarVector position, PlanarVector currentDirection, float roll);
    }
}
