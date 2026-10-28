using ZW.Logic.Interfaces.Handlers.Application;
using ZW.Views.Interfaces.Lifecycle;

namespace ZW.Presenters.Presenters.Lifecycle
{
    public class ApplicationLifecyclePresenter
    {
        public ApplicationLifecyclePresenter(IApplicationLifecycleView view, IApplicationLifecycleHandler[] handlers)
        {
            view.SetListeners(
                paused =>
                {
                    foreach (var handler in handlers)
                    {
                        handler.OnApplicationPause(paused);
                    }
                },
                () =>
                {
                    foreach (var handler in handlers)
                    {
                        handler.OnApplicationQuit();
                    }
                });
        }
    }
}
