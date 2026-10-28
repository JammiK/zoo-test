using JG.Core.Installers.Containers;
using ZW.Logic.Services.Environment;
using ZW.Logic.Services.FoodChains;
using ZW.Logic.Services.Levels;
using ZW.Logic.Services.Movements;
using ZW.Logic.Services.Spawns;
using ZW.Logic.Streams.Animals;
using ZW.Logic.Systems.FoodChains;
using ZW.Logic.Systems.Movements;
using ZW.Logic.Systems.Spawns;

namespace ZW.Logic.Containers
{
    public static class LogicContainer
    {
        public static readonly DiContainerConfig Container = new();

        static LogicContainer()
        {
            InstallStreams();
            InstallSystems();
            InstallServices();
        }

        private static void InstallStreams()
        {
            Container.SelfBind<AnimalSpawnedStream>();
            Container.SelfBind<AnimalMotionStream>();
            Container.SelfBind<AnimalEatenStream>();
            Container.SelfBind<AnimalDiedStream>();
        }

        private static void InstallSystems()
        {
            Container.SelfBind<AnimalSpawnSystem>();
            Container.SelfBind<AnimalMovementSystem>();
            Container.SelfBind<AnimalPositionSyncSystem>();
            Container.SelfBind<AnimalCollisionSystem>();
            Container.SelfBind<AnimalDeathSystem>();
        }

        private static void InstallServices()
        {
            Container.SelfBind<EnvironmentService>();
            Container.SelfBind<RandomDirectionService>();
            Container.SelfBind<LevelAreaService>();
            Container.SelfBind<AnimalSpawnPickService>();
            Container.SelfBind<JumpMovementStrategy>();
            Container.SelfBind<LinearMovementStrategy>();
            Container.SelfBind<PredationService>();
            Container.SelfBind<AnimalDuelService>();
        }
    }
}
