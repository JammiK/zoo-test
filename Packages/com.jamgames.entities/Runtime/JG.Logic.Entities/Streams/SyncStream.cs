using JG.Logic.Entities.Interfaces.Streams;
using JG.Logic.Streams;

namespace JG.Logic.Entities.Streams
{
    public class SyncStream<T> : StreamBase<T>, ISyncStream<T>
    {
    }
}