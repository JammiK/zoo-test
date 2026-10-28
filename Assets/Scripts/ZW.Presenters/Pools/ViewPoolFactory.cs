using Core.JG.Presenters.Core.Interfaces.GameAssets;
using UnityEngine;
using ZW.Presenters.Interfaces.Pools;
using ZW.Views.Interfaces.Common;

namespace ZW.Presenters.Pools
{
    public class ViewPoolFactory : IViewPoolFactory
    {
        private readonly IInstantiateSystem _instantiateSystem;

        public ViewPoolFactory(IInstantiateSystem instantiateSystem)
        {
            _instantiateSystem = instantiateSystem;
        }

        public IObjectPool<T> Create<T>(string address, Transform container) where T : class, IPoolableView
        {
            return new AddressableViewPool<T>(_instantiateSystem, address, container);
        }
    }
}
