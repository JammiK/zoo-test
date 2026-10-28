using System;
using System.Threading;
using System.Threading.Tasks;
using Core.JG.Presenters.Core.Interfaces.GameAssets;
using Core.JG.Views.UI.Core.Interfaces.Common;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets; 
using Object = UnityEngine.Object;

namespace Core.JG.Presenters.Core.Services.GameAssets
{
    public class InstantiateSystem : IInstantiateSystem
    {
        readonly IAddressableLoadService _addressableLoadService;
        readonly Subject<GameObject> _instantiateCallback = new Subject<GameObject>();

        public InstantiateSystem(IAddressableLoadService addressableLoadService = null)
        {
            _addressableLoadService = addressableLoadService;
        }

        public IObservable<GameObject> InstantiateCallback => _instantiateCallback;

        public T Instantiate<T>(IInstantiateContainerView instantiateContainerView)
        {
            var spawnedObject = Instantiate(instantiateContainerView);
            var component = spawnedObject.GetComponent<T>();
            if (component == null)
            {
                Object.Destroy(spawnedObject);
                Debug.LogError($"Unable to GetComponent<{nameof(T)}> in {spawnedObject.name}");
                return default;
            }

            _instantiateCallback.OnNext(spawnedObject);
            return component;
        }

        public T InstantiateContainer<T>(IDynamicInstantiateContainerView<T> instantiateContainerView, string id) where T : IDestroyable
        {
            var view = instantiateContainerView.GetView(id);
            if (view == null)
            {
                view = Instantiate<T>(instantiateContainerView);
                instantiateContainerView.Register(id, view);
            }

            return view;
        }

        public T InstantiateContainer<T, U>(IDynamicContainerView<T, U> instantiateContainerView, U id) where T : IDestroyable, IRemovable where U : IComparable
        {
            var view = instantiateContainerView.GetView(id);
            if (view == null)
            {
                view = Instantiate<T>(instantiateContainerView);
                instantiateContainerView.Register(id, view);
            }

            return view;
        }

        public T InstantiateContainer<T, U>(IDynamicContainerView<T, U> instantiateContainerView, GameObject customPrefab, U id) where T : IDestroyable, IRemovable where U : IComparable
        {
            var view = instantiateContainerView.GetView(id);
            if (view == null)
            {
                if (instantiateContainerView.Container == null)
                {
                    throw new ArgumentException($"Unable to instantiate to Null-container");
                }
                
                view = Instantiate<T>(customPrefab, instantiateContainerView.Container);
                instantiateContainerView.Register(id, view);
            }

            return view;
        }

        public T Instantiate<T>(GameObject prefab, Transform container)
        {
            var gameObject = Object.Instantiate(prefab, container);
            var componentInChildren = gameObject.GetComponentInChildren<T>();
            if (componentInChildren == null)
            {
                Object.Destroy(gameObject);
                return default;
            }

            _instantiateCallback.OnNext(gameObject);
            return componentInChildren;
        }

        public GameObject Instantiate(GameObject prefab, Transform container)
        {
            var gameObject = Object.Instantiate(prefab, container);
            _instantiateCallback.OnNext(gameObject);
            return gameObject;
        }

        public async UniTask<GameObject> Instantiate(AssetReference prefab, Transform container, CancellationToken cancellationToken)
        {
            var gameObject = await Addressables.InstantiateAsync(prefab, container).WithCancellation(cancellationToken);
            _instantiateCallback.OnNext(gameObject);

            return gameObject;
        }

        public async UniTask<T> Instantiate<T>(AssetReference prefab, Transform container, CancellationToken cancellationToken)
        {
            var gameObject = await Instantiate(prefab, container, cancellationToken);
            if (gameObject)
            {
                var component = gameObject.GetComponentInChildren<T>(true);
                if (component == null)
                {
                    Debug.LogError($"Unable to GetComponent<{nameof(T)}> in {gameObject.name}");
                    Object.Destroy(gameObject);
                    return default;
                }

                return component;
            }

            return default;
        }

        public async UniTask<T> Instantiate<T>(string prefabAddress, Transform container, CancellationToken cancellationToken)
        {
            var gameObject = await Addressables.InstantiateAsync(prefabAddress, container).WithCancellation(cancellationToken);
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Unable to GetComponent<{nameof(T)}> in {gameObject.name}");
                Object.Destroy(gameObject);
                return default;
            }
            
            _instantiateCallback.OnNext(gameObject);
            return component;
        }

        public GameObject Instantiate(IInstantiateContainerView instantiateContainerView)
        {
            if (instantiateContainerView.Prefab == null)
            {
                throw new ArgumentException($"Unable to instantiate Null-object");
            }
            
            if (instantiateContainerView.Container == null)
            {
                throw new ArgumentException($"Unable to instantiate to Null-container");
            }

            var gameObject = Object.Instantiate(instantiateContainerView.Prefab, instantiateContainerView.Container);
            _instantiateCallback.OnNext(gameObject);
            return gameObject;
        }
        
        public IObservable<GameObject> InstantiateAsync(string path)
        {
            var subject = new Subject<GameObject>();
            Addressables.InstantiateAsync(path).Completed += handle =>
            {
                if (handle.Result != null)
                {
                    _instantiateCallback.OnNext(handle.Result);
                }

                subject.OnNext(handle.Result);
                subject.OnCompleted();
            };

            return subject;
        }
        
        public async Task<GameObject> Instantiate(string path, Transform container)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            Object prefab = null;
            if (_addressableLoadService != null)
            {
                prefab = _addressableLoadService.GetFromCache(path);
            }
            else
            {
                prefab = await Addressables.LoadAssetAsync<GameObject>(path);
            }

            var prefabObject = prefab as GameObject;
            if (container == null)
            {
                if (prefab != null)
                {
                    return Object.Instantiate(prefabObject);
                }

                var result = await Addressables.InstantiateAsync(path).Task;
                _instantiateCallback.OnNext(result);
                return result;
            }

            GameObject gameObject;
            if (prefabObject == null)
            {
                gameObject = await Addressables.InstantiateAsync(path, container).Task;
            }
            else
            {
                gameObject = Object.Instantiate(prefabObject, container);
            }

            _instantiateCallback.OnNext(gameObject);
            return gameObject;
        }
    }
}