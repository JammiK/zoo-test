using JG.Core.Installers.Containers;
using ZW.Logic.Services.Environment;
using ZW.Logic.Services.Movements;

namespace ZW.Logic.Containers
{
    public static class LogicContainer
    {
        public static readonly DiContainerConfig Container = new();

        static LogicContainer()
        {
            InstallServices();
        }

        private static void InstallServices()
        {
            Container.SelfBind<EnvironmentService>();
            Container.SelfBind<RandomDirectionService>();
        }
    }
}
