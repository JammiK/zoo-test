using System;
using JG.Core.Installers.Containers;
using Zenject;

namespace JG.Core.Installers
{
    public static class ZenjectExtension
    {
        public static void SelfBind<T>(this DiContainer container, bool lazy = false)
        {
            var binder = container.BindInterfacesAndSelfTo<T>().AsSingle();
            if (!lazy)
            {
                binder.NonLazy();
            }
        }
        
        public static void SelfBind(this DiContainer container, Type type, bool lazy = false)
        {
            var binder = container.BindInterfacesAndSelfTo(type).AsSingle();
            if (!lazy)
            {
                binder.NonLazy();
            }
        }

        public static void ViewBind<T>(this DiContainer container)
        {
            container.Bind<T>().FromComponentsInHierarchy(includeInactive: true).AsSingle();
        }
        
        public static void ViewBind(this DiContainer container, Type type)
        {
            container.Bind(type).FromComponentsInHierarchy(includeInactive: true).AsSingle();
        }

        public static void BindConfig(this DiContainer container, DiContainerConfig config)
        {
            for (var i = 0; i < config.Bindings.Count; i++)
            {
                var typeBindingConfig = config.Bindings[i];
                if (typeBindingConfig.IsSceneObject)
                {
                    container.ViewBind(typeBindingConfig.Type);
                }
                else
                {
                    container.SelfBind(typeBindingConfig.Type, typeBindingConfig.Lazy);
                }
            }
        }
    }
}