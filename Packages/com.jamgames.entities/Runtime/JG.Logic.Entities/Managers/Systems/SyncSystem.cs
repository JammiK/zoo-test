using JG.DataManagers.Entities.Interfaces.Systems;
using JG.Logic.Entities.Interfaces.Streams;

namespace JG.Logic.Entities.Managers.Systems
{
    public class SyncSystem<T> : ISyncSystem<T>
    {
        readonly ISyncStream<T> _syncStream;

        public SyncSystem(ISyncStream<T> syncStream)
        {
            _syncStream = syncStream;
        }

        public void Sync(T flag)
        {
            _syncStream.OnNext(flag);
        }
    }
}