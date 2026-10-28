namespace ZW.Data.Interfaces.Entities.Movements
{
    public interface IJumpMovementEntity : IMovementEntity
    {
        float Distance { get; }
        float Height { get; }
        float Interval { get; }
    }
}
