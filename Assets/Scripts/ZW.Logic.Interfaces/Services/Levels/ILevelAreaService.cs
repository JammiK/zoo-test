using ZW.Data.Models.Types;

namespace ZW.Logic.Interfaces.Services.Levels
{
    public interface ILevelAreaService
    {
        bool IsInside(PlanarVector point);
        PlanarVector InwardNormal(PlanarVector point);
        PlanarVector RandomPoint();
    }
}
