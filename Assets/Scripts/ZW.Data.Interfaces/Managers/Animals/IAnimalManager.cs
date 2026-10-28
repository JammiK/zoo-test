using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Models.Types;

namespace ZW.Data.Interfaces.Managers.Animals
{
    public interface IAnimalManager
    {
        void SetPosition(IAnimalModel animal, PlanarVector position);
        void SetDirection(IAnimalModel animal, PlanarVector direction);
        void SetMoveCooldown(IAnimalModel animal, float cooldown);
        void SetKeepDirection(IAnimalModel animal, bool keep);
        void Kill(IAnimalModel animal);
    }
}
