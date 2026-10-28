using System;
using System.Threading;
using UnityEngine;

namespace Core.JG.Views.UI.Core.Interfaces.Common
{
    public interface IViewBase : IDisposable
    {
        event Action OnDestroyed;
        CancellationToken DisposeToken { get; }
        void Initialize();
        void SelfDestroy();
        void SetOnStart(Action<IViewBase> onStart);
        GameObject GameObject { get; }
    }
}