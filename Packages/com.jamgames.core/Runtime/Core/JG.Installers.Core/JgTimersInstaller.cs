using Core.JG.Data.Providers.Time;
using Core.JG.Presenters.Core.Views.Timers;
using Core.JG.Views.UI.Core.Interfaces.Timers;
using JG.Core.Installers.Containers;

namespace Core.JG.Installers.Core
{
    public class JgTimersInstaller
    {
        public static readonly DiContainerConfig Config = new();

        static JgTimersInstaller()
        {
            Config.SelfBind<TimeProvider>();
            
            Config.SelfBind<TimerPresenter>();
            
            Config.ViewBind<ITimerView>();
        }
    }
}