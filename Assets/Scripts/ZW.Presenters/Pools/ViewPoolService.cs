using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZW.Presenters.Interfaces.Pools;
using ZW.Views.Interfaces.Common;

namespace ZW.Presenters.Pools
{
    public class ViewPoolService : IViewPoolService
    {
        private readonly IViewPoolFactory _poolFactory;
        private readonly Dictionary<(string, System.Type), object> _pools = new();

        public ViewPoolService(IViewPoolFactory poolFactory)
        {
            _poolFactory = poolFactory;
        }

        public UniTask<T> Get<T>(string address, Transform container, CancellationToken token) where T : class, IPoolableView
        {
            return GetPool<T>(address, container).Get(token);
        }

        public void Release<T>(string address, T view) where T : class, IPoolableView
        {
            GetPool<T>(address, null).Release(view);
        }

        private IObjectPool<T> GetPool<T>(string address, Transform container) where T : class, IPoolableView
        {
            var key = (address, typeof(T));
            if (!_pools.TryGetValue(key, out var pool))
            {
                pool = _poolFactory.Create<T>(address, container);
                _pools[key] = pool;
            }

            return (IObjectPool<T>)pool;
        }
    }
}
