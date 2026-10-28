using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.JG.Views.Actors.Core
{
    public class GameObjectPool<T> where T : Component
    {
        readonly T prefab_;
        readonly Transform parent_;
        readonly List<T> list_;
        int capacity_;
        Action<T> initializer_;

        public GameObjectPool(int capacity, T prefab, Transform parent, Action<T> initializer = null)
        {
            prefab_ = prefab;
            parent_ = parent;
            capacity_ = capacity;
            initializer_ = initializer;
            list_ = new List<T>(capacity);
        }

        public void CreateObjects()
        {
            for (int i = 0; i < capacity_; i++)
            {
                AddPoolObject();
            }
        }

        public void CreateObject()
        {
            AddPoolObject();
        }

        public void FreeObjects()
        {
            for (int i = 0; i < capacity_; i++)
            {
                list_[i].gameObject.SetActive(false);
            }
        }

        public T GetObject()
        {
            for (int i = 0; i < capacity_; i++)
            {
                T instance = list_[i];
                if (!instance.gameObject.activeSelf)
                {
                    instance.gameObject.SetActive(true);
                    return instance;
                }
            }

            int objectIndex = capacity_;

            capacity_ *= 2;
            list_.Capacity = capacity_;

            for (int i = objectIndex; i < capacity_; i++)
            {
                AddPoolObject();
            }

            list_[objectIndex].gameObject.SetActive(true);
            return list_[objectIndex];
        }

        public void ReturnObject(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        private void AddPoolObject()
        {
            T instance = Object.Instantiate(prefab_, parent_, false);
            if (initializer_ != null)
                initializer_(instance);
            instance.gameObject.SetActive(false);
            list_.Add(instance);
        }
    }
}
