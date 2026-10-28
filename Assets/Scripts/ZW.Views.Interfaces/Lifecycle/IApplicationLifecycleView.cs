using System;

namespace ZW.Views.Interfaces.Lifecycle
{
    public interface IApplicationLifecycleView
    {
        void SetListeners(Action<bool> onPause, Action onQuit);
    }
}
