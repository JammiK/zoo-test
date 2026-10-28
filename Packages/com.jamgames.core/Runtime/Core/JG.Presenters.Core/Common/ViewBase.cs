using System;
using System.Threading;
using Core.JG.Views.UI.Core.Interfaces.Common;
using UnityEngine;

namespace JG.Views.UI.Core.Common
{
    public class ViewBase : MonoBehaviour, IViewBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        private Action<IViewBase> _onStart;
        
        public virtual void Dispose()
        {
            _cancellationTokenSource.Cancel();
            OnDestroyed?.Invoke();
        }

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            _onStart?.Invoke(this);
        }

        public event Action OnDestroyed;
        public CancellationToken DisposeToken => _cancellationTokenSource.Token;
        public virtual void Initialize()
        {
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void SelfDestroy()
        {
            Destroy(gameObject);
        }

        public void SetOnStart(Action<IViewBase> onStart)
        {
            _onStart = onStart;
        }

        public GameObject GameObject => gameObject;
    }
}