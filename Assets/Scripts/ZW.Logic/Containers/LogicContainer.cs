using JG.Core.Installers.Containers;
using ZW.Logic.Services.Environment;
using ZW.Logic.Services.Movements;
using ZW.Logic.Streams.Animals;

namespace ZW.Logic.Containers
{
    public static class LogicContainer
    {
        public static readonly DiContainerConfig Container = new();

        static LogicContainer()
        {
            InstallStreams();
            InstallServices();
        }

        private static void InstallStreams()
        {
            Container.SelfBind<AnimalSpawnedStream>();
            Container.SelfBind<AnimalMotionStream>();
            Container.SelfBind<AnimalEatenStream>();
            Container.SelfBind<AnimalDiedStream>();
        }

        private static void InstallServices()
        {
            Container.SelfBind<EnvironmentService>();
            Container.SelfBind<RandomDirectionService>();
        }
    }
}
