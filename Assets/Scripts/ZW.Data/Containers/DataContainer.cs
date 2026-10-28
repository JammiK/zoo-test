using Core.JG.Data.Services;
using JG.Core.Installers.Containers;
using ZW.Data.Factories.Animals;
using ZW.Data.Managers.Animals;
using ZW.Data.Managers.Stats;
using ZW.Data.Managers.Zoo;
using ZW.Data.Providers.Animals;
using ZW.Data.Providers.Stats;

namespace ZW.Data.Containers
{
    public static class DataContainer
    {
        public static readonly DiContainerConfig Container = new();

        static DataContainer()
        {
            Container.SelfBind<RandomService>();

            Container.SelfBind<AnimalFactory>();

            Container.SelfBind<AnimalManager>();
            Container.SelfBind<ZooManager>();
            Container.SelfBind<DeathStatsManager>();

            Container.SelfBind<AnimalProvider>();
            Container.SelfBind<DeathStatsProvider>();
        }
    }
}
