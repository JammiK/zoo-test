using System.Collections.Generic;
using System.Linq;
using Core.JG.Views.UI.Core.Interfaces.Common;
using UnityEngine;

namespace Core.JG.Views.UI.Core.Common
{
    public abstract class DynamicInstantiateContainerViewBase<T> : MonoBehaviour, IDynamicInstantiateContainerView<T> where T : class, IDestroyable, IRemovable
    {
        [SerializeField] GameObject _prefab;
        [SerializeField] Transform _container;
        public GameObject Prefab => _prefab;
        public Transform Container => _container;

        readonly protected Dictionary<string, T> SpawnedObjects = new Dictionary<string, T>();
        public void Register(string id, T view)
        {
            if (SpawnedObjects.ContainsKey(id))
            {
                SpawnedObjects[id].SelfDestroy();
            }

            SpawnedObjects[id] = view;
        }
        public T GetView(string id)
        {
            return SpawnedObjects.ContainsKey(id) ? SpawnedObjects[id] : default;
        }

        public IEnumerable<T> GetViews()
        {
            return SpawnedObjects.Values;
        }

        public void Clear()
        {
            foreach (var keyValuePair in SpawnedObjects)
            {
                keyValuePair.Value.SelfDestroy();
            }
            
            SpawnedObjects.Clear();
        }

        public void Destroy(string id)
        {
            if (SpawnedObjects.ContainsKey(id))
            {
                SpawnedObjects[id].SelfDestroy();
            }
        }

        public void DestroyAllMarkedForRemove()
        {
            foreach (var obj in SpawnedObjects.Values.Where(x => x.MarkToRemove))
            {
                obj.SelfDestroy();
            }
            
        }

        public void MarkAllForRemove()
        {
            foreach (var value in SpawnedObjects.Values)
            {
                value.MarkToRemove = true;
            }
        }

        void OnValidate()
        {
            if (_container == null)
            {
                _container = transform;
            }

            if (_prefab != null && _prefab.GetComponent<T>() == null)
            {
                _prefab = null;
                Debug.LogError($"Unable to get component {typeof(T)} in prefab");
            } 
        }
    }
}