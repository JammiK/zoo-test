using ZW.Data.Models.Types;

namespace ZW.Logic.Models.Movements
{
    public readonly struct MovementContext
    {
        public readonly float DeltaTime;
        public readonly bool IsInsideArea;
        public readonly PlanarVector ReturnDirection;

        public MovementContext(float deltaTime, bool isInsideArea, PlanarVector returnDirection)
        {
            DeltaTime = deltaTime;
            IsInsideArea = isInsideArea;
            ReturnDirection = returnDirection;
        }
    }
}
