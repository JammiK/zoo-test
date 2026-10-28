using System.Threading;
using Cysharp.Threading.Tasks;

namespace ZW.Presenters.Interfaces.Pools
{
    public interface IObjectPool<T>
    {
        int FreeCount { get; }
        UniTask<T> Get(CancellationToken token);
        void Release(T item);
    }
}
