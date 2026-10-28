using ZW.Data.Models.Types;

namespace ZW.Logic.Interfaces.Services.Movements
{
    public interface IRandomDirectionService
    {
        PlanarVector Next();
    }
}
