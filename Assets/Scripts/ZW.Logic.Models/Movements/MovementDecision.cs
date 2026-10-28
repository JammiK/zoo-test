using ZW.Data.Models.Types;

namespace ZW.Logic.Models.Movements
{
    public readonly struct MovementDecision
    {
        public readonly PlanarVector Direction;
        public readonly float Cooldown;
        public readonly AnimalMotion? Motion;

        public MovementDecision(PlanarVector direction, float cooldown, AnimalMotion? motion)
        {
            Direction = direction;
            Cooldown = cooldown;
            Motion = motion;
        }
    }
}
