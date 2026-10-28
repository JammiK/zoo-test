namespace JG.DataManagers.Entities.Interfaces.Systems
{
    public interface ISyncSystem<T>
    {
        void Sync(T flag);
    }
}