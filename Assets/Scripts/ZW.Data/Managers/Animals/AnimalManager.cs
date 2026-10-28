using System;
using ZW.Data.Interfaces.Managers.Animals;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Interfaces.Types.Animals;
using ZW.Data.Models.Types;

namespace ZW.Data.Managers.Animals
{
    public class AnimalManager : IAnimalManager
    {
        public void SetPosition(IAnimalModel animal, PlanarVector position)
        {
            animal.Position = position;
        }

        public void SetDirection(IAnimalModel animal, PlanarVector direction)
        {
            animal.Direction = direction;
        }

        public void SetMoveCooldown(IAnimalModel animal, float cooldown)
        {
            animal.MoveCooldown = Math.Max(0f, cooldown);
        }

        public void Kill(IAnimalModel animal)
        {
            animal.State = AnimalState.Dead;
        }
    }
}
