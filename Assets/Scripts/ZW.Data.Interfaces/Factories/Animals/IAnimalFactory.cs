using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Models.Types;

namespace ZW.Data.Interfaces.Factories.Animals
{
    public interface IAnimalFactory
    {
        IAnimalModel Create(int uid, IAnimalEntity entity, PlanarVector position, PlanarVector direction);
    }
}
