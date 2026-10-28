using Core.JG.Installers.Core;
using Core.JG.Logic.Core.Interfaces.Systems.Core;
using Core.JG.Logic.Core.Systems.Core;
using JG.Core.Installers;
using JG.Core.Installers.Containers;
using JG.Installers.Entities;
using Zenject;
using ZW.Data.Containers;
using ZW.Logic.Containers;
using ZW.Presenters.Containers;
using ZW.Views.Containers;

namespace ZW.Installers
{
    public class GameInstaller : MonoInstaller
    {
        private static readonly string[] EntityAssemblyPrefixes = { "ZW", "JG" };

        public override void InstallBindings()
        {
            var entityBindings = new DiContainerConfig();
            EntityRegistrator.Bind(entityBindings, EntityAssemblyPrefixes);

            Container.BindConfig(entityBindings);
            Container.BindConfig(JgEntitiesInstaller.Config);
            Container.BindConfig(JgCoreInstaller.Config);
            Container.BindConfig(DataContainer.Container);
            Container.BindConfig(LogicContainer.Container);
            Container.BindConfig(PresentersContainer.Container);
            Container.BindConfig(ViewsContainer.Container);

            Container.Bind<IUpdateSystem>().To<UpdateSystem>().FromNewComponentOnNewGameObject().AsSingle();
        }
    }
}
