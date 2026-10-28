using ZW.Data.Models.Types;

namespace ZW.Logic.Models.Movements
{
    public readonly struct AnimalMotion
    {
        public readonly PlanarVector Velocity;
        public readonly float VerticalVelocity;

        public AnimalMotion(PlanarVector velocity, float verticalVelocity)
        {
            Velocity = velocity;
            VerticalVelocity = verticalVelocity;
        }
    }
}
