using System;
using System.Threading;
using System.Threading.Tasks;
using Core.JG.Views.UI.Core.Interfaces.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.JG.Presenters.Core.Interfaces.GameAssets
{
    public interface IInstantiateSystem
    {
        IObservable<GameObject> InstantiateCallback { get; }
        T Instantiate<T>(IInstantiateContainerView instantiateContainerView);
        [Obsolete("Use IDynamicContainerView")]
        T InstantiateContainer<T>(IDynamicInstantiateContainerView<T> instantiateContainerView, string id) where T : IDestroyable;
        T InstantiateContainer<T, U>(IDynamicContainerView<T, U> instantiateContainerView, U id) where T : IDestroyable, IRemovable where U : IComparable;
        T InstantiateContainer<T, U>(IDynamicContainerView<T, U> instantiateContainerView, GameObject customPrefab, U id) where T : IDestroyable, IRemovable where U : IComparable;
        T Instantiate<T>(GameObject prefab, Transform container);
        GameObject Instantiate(GameObject prefab, Transform container);
        UniTask<GameObject> Instantiate(AssetReference prefab, Transform container, CancellationToken cancellationToken);
        UniTask<T> Instantiate<T>(AssetReference prefab, Transform container, CancellationToken cancellationToken);
        UniTask<T> Instantiate<T>(string prefabAddress, Transform container, CancellationToken cancellationToken);
        GameObject Instantiate(IInstantiateContainerView instantiateContainerView);
        IObservable<GameObject> InstantiateAsync(string path);
        Task<GameObject> Instantiate(string path, Transform container);
    }
}