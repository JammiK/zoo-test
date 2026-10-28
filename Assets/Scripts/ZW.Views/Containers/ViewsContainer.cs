using JG.Core.Installers.Containers;
using ZW.Views.Interfaces.Actors.World;
using ZW.Views.Interfaces.Lifecycle;
using ZW.Views.Interfaces.UI.Stats;

namespace ZW.Views.Containers
{
    public static class ViewsContainer
    {
        public static readonly DiContainerConfig Container = new();

        static ViewsContainer()
        {
            Container.ViewBind<IWorldContainerView>();
            Container.ViewBind<ICameraView>();
            Container.ViewBind<IPhysicsProbeView>();
            Container.ViewBind<IDeathCounterView>();
            Container.ViewBind<IApplicationLifecycleView>();
        }
    }
}
