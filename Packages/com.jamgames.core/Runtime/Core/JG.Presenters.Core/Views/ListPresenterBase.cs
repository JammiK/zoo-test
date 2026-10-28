using System;
using System.Collections.Generic;
using Core.JG.Presenters.Core.Interfaces.Views;
using Core.JG.Views.UI.Core.Interfaces;
using Core.JG.Views.UI.Core.Interfaces.Buttons;
using UniRx;
using UnityEngine;
using Object = System.Object;

namespace Core.JG.Presenters.Core.Views
{
    public abstract class ListPresenterBase<T, TU> : IListPresenter<T, TU> where T : IDynamicView<TU>
    {
        readonly IViewPresenter<T, TU> _viewPresenter;
        readonly IListContainerView[] _listContainerViews;
        
        readonly Dictionary<IListContainerView, Dictionary<TU, T>> _spawnedViews = new Dictionary<IListContainerView, Dictionary<TU, T>>();

        bool _defaultModelsAlreadySet;
        
        protected ListPresenterBase(IViewPresenter<T, TU> viewPresenter,
            IListContainerView[] listContainerViews,
            IObservable<TU> addStream = null,
            IObservable<TU> removeStream = null,
            IEnumerable<TU> defaultModels = null
            )
        {
            _viewPresenter = viewPresenter;
            _listContainerViews = listContainerViews;
            addStream?.Subscribe(HandleAdd);
            removeStream?.Subscribe(HandleRemove);
            if (defaultModels != null)
            {
                SetDefaultCollection(defaultModels);
            }
        }

        protected void SetDefaultCollection(IEnumerable<TU> models)
        {
            if (_defaultModelsAlreadySet)
            {
                Debug.Log($"Default model with type '{nameof(TU)}' for view '{nameof(T)}' already set.");
            }
            
            foreach (var defaultModel in models)
            {
                HandleAdd(defaultModel);
            }

            _defaultModelsAlreadySet = true;
        }

        void HandleAdd(TU model)
        {
            foreach (var listContainerView in _listContainerViews)
            {
                var spawnedView = GetSpawnedView(listContainerView, model);
                if (spawnedView != null)
                {
                    _viewPresenter.Present(spawnedView, model);
                    return;
                }

                var spawnedObject = UnityEngine.Object.Instantiate(listContainerView.Prefab, listContainerView.Container);
                spawnedView = spawnedObject.GetComponent<T>();
                if (spawnedView == null)
                {
                    UnityEngine.Object.Destroy(spawnedObject);
                    return;
                }
                
                _viewPresenter.Present(spawnedView, model);
                if (!_spawnedViews.ContainsKey(listContainerView))
                {
                    _spawnedViews.Add(listContainerView, new Dictionary<TU, T>());
                }
                
                _spawnedViews[listContainerView].Add(model, spawnedView);
                PostSpawn(spawnedObject, spawnedView, model);
            }
        }

        T GetSpawnedView(IListContainerView containerView, TU model)
        {
            if (!_spawnedViews.ContainsKey(containerView))
            {
                return default;
            }

            var map = _spawnedViews[containerView];
            if (!map.ContainsKey(model))
            {
                return default;
            }

            return map[model];
        }

        void HandleRemove(TU model)
        {
            foreach (var listContainerView in _listContainerViews)
            {
                var spawnedView = GetSpawnedView(listContainerView, model);
                if (spawnedView == null)
                {
                    return;
                }
                
                PreDestroy(spawnedView, model);
                spawnedView.SelfDestroy();
                _spawnedViews[listContainerView].Remove(model);
            }
        }

        protected virtual void PreDestroy(T view, TU model)
        {
            //empty   
        }

        protected virtual void PostSpawn(GameObject gameObject, T view, TU model)
        {
            //empty
        }
    }
}