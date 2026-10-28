using System;
using UnityEngine;
using ZW.Views.Interfaces.Lifecycle;

namespace ZW.Views.Lifecycle
{
    public class ApplicationLifecycleView : MonoBehaviour, IApplicationLifecycleView
    {
        private Action<bool> _onPause;
        private Action _onQuit;

        public void SetListeners(Action<bool> onPause, Action onQuit)
        {
            _onPause = onPause;
            _onQuit = onQuit;
        }

        private void OnApplicationPause(bool paused)
        {
            _onPause?.Invoke(paused);
        }

        private void OnApplicationQuit()
        {
            _onQuit?.Invoke();
        }
    }
}
