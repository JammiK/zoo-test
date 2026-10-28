namespace JG.Data.Entities.Interfaces.Systems.Saves
{
    public interface ISaveSystem
    {
        void RequestSave();
        void SetLocked(bool locked);
        void Save();
    }
}
