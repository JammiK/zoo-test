using System;
using Core.JG.Presenters.Core.Interfaces.GameAssets;
using Core.JG.Presenters.Core.Interfaces.Systems.DynamicPresenters;
using Core.JG.Presenters.Core.Interfaces.Views;
using Core.JG.Views.UI.Core.Interfaces.Common;
using UniRx;
using UnityEngine;

namespace JG.Presenters.Core.Systems.DynamicPresenters
{
    public class DynamicPresenterSystem : IDynamicPresenterSystem, IDisposable
    {
        private readonly IViewPresenter[] _viewPresenters;
        IDisposable _disposableCallback = null;
        public DynamicPresenterSystem(IInstantiateSystem instantiateSystem,
            IViewPresenter[] viewPresenters,
            IViewBase[] viewBases)
        {
            _viewPresenters = viewPresenters;
            _disposableCallback = instantiateSystem.InstantiateCallback.Subscribe(OnInstantiate);
            
            foreach (var viewBase in viewBases)
            {
                InitializeView(viewBase);
            }
        }

        private void OnInstantiate(GameObject gameObject)
        {
            var views = gameObject.GetComponentsInChildren<IViewBase>(true);
            foreach (var viewBase in views)
            {
                viewBase.SetOnStart(InitializeView);
            }
        }
        
        void InitializeView(IViewBase viewBase)
        {
            var viewType = viewBase.GetType();
            var interfaces = viewType.GetInterfaces();
            foreach (var iface in interfaces)
            {
                if (typeof(IViewBase).IsAssignableFrom(iface) && iface != typeof(IViewBase))
                {
                    var presenterInterface = typeof(IViewPresenter<>).MakeGenericType(iface);
                    foreach (var presenter in _viewPresenters)
                    {
                        if (presenterInterface.IsInstanceOfType(presenter))
                        {
                            var presentMethod = presenterInterface.GetMethod("Present");
                            if (presentMethod != null)
                            {
                                presentMethod.Invoke(presenter, new object[] { viewBase });
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            _disposableCallback?.Dispose();
            _disposableCallback = null;
        }
    }
}