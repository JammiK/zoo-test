using System;
using System.Collections.Generic;
using System.Linq;
using Core.JG.Views.UI.Core.Interfaces.Common;
using UnityEngine;

namespace Core.JG.Views.UI.Core.Common
{

    public class DynamicContainerViewBase<T, U> : MonoBehaviour, IDynamicContainerView<T, U>
        where T : IDestroyable, IRemovable 
        where U : IComparable
    {
        [SerializeField] GameObject _prefab;
        [SerializeField] Transform _container;
        public GameObject Prefab => _prefab;
        public Transform Container => _container;

        readonly protected Dictionary<U, T> SpawnedObjects = new Dictionary<U, T>();

        public void Register(U id, T view)
        {
            if (SpawnedObjects.ContainsKey(id))
            {
                SpawnedObjects[id].SelfDestroy();
            }

            SpawnedObjects[id] = view;
        }

        public T GetView(U id)
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

        public void Destroy(U id)
        {
            if (SpawnedObjects.ContainsKey(id))
            {
                SpawnedObjects[id].SelfDestroy();
            }
        }

        public void DestroyAllMarkedForRemove()
        {
            List<U> keysToRemove = null;
            foreach (var keyValuePair in SpawnedObjects)
            {
                if (keyValuePair.Value.MarkToRemove)
                {
                    keyValuePair.Value.SelfDestroy();
                    if (keysToRemove == null)
                    {
                        keysToRemove = new List<U>();
                    }
                    
                    keysToRemove.Add(keyValuePair.Key);
                }
            }

            if (keysToRemove != null)
            {
                foreach (var key in keysToRemove)
                {
                    SpawnedObjects.Remove(key);
                }
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