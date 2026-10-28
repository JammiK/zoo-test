using Core.JG.Logic.Core.Services.Loaders;
using Core.JG.Presenters.Core.Services.GameAssets;
using Core.JG.Views.UI.Core.Interfaces.Common;
using IR.Presenters.Services.Resources;
using JG.Core.Installers.Containers;
using JG.Presenters.Core.Systems.DynamicPresenters;

namespace Core.JG.Installers.Core
{
    public class JgCoreInstaller
    {
        public static readonly DiContainerConfig Config = new DiContainerConfig();

        static JgCoreInstaller()
        {
            Config.SelfBind<TextLoadService>();
            Config.SelfBind<ResourcesLoadService>();
            Config.SelfBind<InstantiateSystem>();
        }
    }
}