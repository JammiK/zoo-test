using ZW.Data.Interfaces.Models.Animals;

namespace ZW.Data.Interfaces.Managers.Zoo
{
    public interface IZooManager
    {
        int TakeNextUid();
        void AddAnimal(IAnimalModel animal);
        void RemoveAnimal(IAnimalModel animal);
        void SetSpawnCooldown(float cooldown);
    }
}
