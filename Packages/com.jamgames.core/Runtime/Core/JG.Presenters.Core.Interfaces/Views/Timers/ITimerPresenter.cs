using Core.JG.Views.UI.Core.Interfaces.Timers;

namespace Core.JG.Presenters.Core.Interfaces.Views.Timers
{
    public interface ITimerPresenter
    {
        void Register(ITimerView timerView);
    }
}