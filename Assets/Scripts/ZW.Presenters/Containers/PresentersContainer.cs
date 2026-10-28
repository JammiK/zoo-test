using JG.Core.Installers.Containers;
using ZW.Presenters.Presenters.Animals;
using ZW.Presenters.Presenters.Levels;
using ZW.Presenters.Presenters.Lifecycle;
using ZW.Presenters.Presenters.Stats;
using ZW.Presenters.Providers.Animals;
using ZW.Presenters.Services.Spawns;

namespace ZW.Presenters.Containers
{
    public static class PresentersContainer
    {
        public static readonly DiContainerConfig Container = new();

        static PresentersContainer()
        {
            Container.SelfBind<AnimalActorProvider>();
            Container.SelfBind<PhysicsSpawnPointValidator>();

            Container.SelfBind<LevelPresenter>();
            Container.SelfBind<AnimalActorPresenter>();
            Container.SelfBind<AnimalMotionPresenter>();
            Container.SelfBind<TastyLabelPresenter>();
            Container.SelfBind<DeathStatsPresenter>();
            Container.SelfBind<ApplicationLifecyclePresenter>();
        }
    }
}
