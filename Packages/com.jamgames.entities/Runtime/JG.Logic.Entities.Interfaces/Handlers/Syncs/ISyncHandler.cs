namespace JG.Logic.Entities.Interfaces.Handlers.Syncs
{
    public interface ISyncHandler<T>
    {
        T SyncFlag { get; }
        void HandleSync(T syncFlag);
    }
}