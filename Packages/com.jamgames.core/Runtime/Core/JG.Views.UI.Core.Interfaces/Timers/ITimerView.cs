using System;

namespace Core.JG.Views.UI.Core.Interfaces.Timers
{
    public interface ITimerView
    {
        long TargetTime { get; set; }
        bool Enabled { get; set; }
        bool Finished { get; set; }
        void SetTimer(string timer);
        void SetOnDestroy(Action<ITimerView> onDestroy);
        void SetOnEnable(Action<ITimerView> onEnable);
    }
}