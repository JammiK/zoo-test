namespace ZW.Logic.Interfaces.Handlers.Application
{
    public interface IApplicationLifecycleHandler
    {
        void OnApplicationPause(bool paused);
        void OnApplicationQuit();
    }
}
