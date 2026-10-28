using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ZW.Presenters.Interfaces.Pools;

namespace ZW.Presenters.Pools
{
    public abstract class ObjectPoolBase<T> : IObjectPool<T> where T : class
    {
        private readonly Stack<T> _free = new();

        public int FreeCount => _free.Count;

        public async UniTask<T> Get(CancellationToken token)
        {
            var item = _free.Count > 0 ? _free.Pop() : await Create(token);
            if (item != null)
            {
                OnGet(item);
            }

            return item;
        }

        public void Release(T item)
        {
            OnRelease(item);
            _free.Push(item);
        }

        protected abstract UniTask<T> Create(CancellationToken token);

        protected virtual void OnGet(T item)
        {
        }

        protected virtual void OnRelease(T item)
        {
        }
    }
}
