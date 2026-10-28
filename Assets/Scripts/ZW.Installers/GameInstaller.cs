using Core.JG.Installers.Core;
using Core.JG.Logic.Core.Interfaces.Systems.Core;
using Core.JG.Logic.Core.Systems.Core;
using JG.Core.Installers;
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
        public override void InstallBindings()
        {
            EntityRegistrator.Bind(DataContainer.Container, new[] { "ZW", "JG" });

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
