using System.Threading;
using Core.JG.Presenters.Core.Interfaces.GameAssets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZW.Views.Interfaces.Common;

namespace ZW.Presenters.Pools
{
    public class AddressableViewPool<T> : ObjectPoolBase<T> where T : class, IPoolableView
    {
        private readonly IInstantiateSystem _instantiateSystem;
        private readonly string _address;
        private readonly Transform _container;

        public AddressableViewPool(IInstantiateSystem instantiateSystem, string address, Transform container)
        {
            _instantiateSystem = instantiateSystem;
            _address = address;
            _container = container;
        }

        protected override UniTask<T> Create(CancellationToken token)
        {
            return _instantiateSystem.Instantiate<T>(_address, _container, token);
        }

        protected override void OnGet(T item)
        {
            item.SetActive(true);
        }

        protected override void OnRelease(T item)
        {
            item.SetActive(false);
        }
    }
}
