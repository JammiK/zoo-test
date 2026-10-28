using System;
using System.Linq;
using JG.Core.Installers.Containers;
using JG.Data.Entities.Factories;
using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Entities.Interfaces.Factories.Contexts.Common;
using JG.Data.Entities.Interfaces.Factories.Models.Common;
using JG.Data.Entities.Models;
using JG.Data.Entities.Repositories.Common;

namespace JG.Installers.Entities
{
    public static class EntityRegistrator
    {
        public static void Bind(DiContainerConfig config, string[] assemblyPrefixes)
        {
            BindDynamicRepositories(config, assemblyPrefixes);
            BindDynamicContextSaveFactories(config, assemblyPrefixes);
            BindDynamicModelSaveFactories(config, assemblyPrefixes);
        }
        
        static void BindDynamicRepositories(DiContainerConfig config, string[] assemblyPrefixes)
        {
            var entityType = typeof(IEntity);
            var editorEntityType = typeof(IEditorEntity<>);
            var entityInterfaces = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => assemblyPrefixes.Any(x => a.FullName.Contains(x)))
                .SelectMany(a => a.GetTypes())
                .Where(type => type != entityType)
                .Where(type => entityType.IsAssignableFrom(type) && !editorEntityType.IsAssignableFrom(type))
                .Where(type => type.IsInterface);

            foreach (var entityInterface in entityInterfaces)
            {
                var makeGenericType = typeof(EntityRepository<>).MakeGenericType(entityInterface);
                config.SelfBind(makeGenericType);
            }
        }

        static void BindDynamicContextSaveFactories(DiContainerConfig config, string[] assemblyPrefixes)
        {
            var factoryType = typeof(ContextFactoryBase<,>);
            var interfaces = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => assemblyPrefixes.Any(x => a.FullName.Contains(x)))
                .SelectMany(a => a.GetTypes())
                .Where(type => type != factoryType)
                .Where(type => typeof(IContextFactory).IsAssignableFrom(type))
                .Where(type => type.IsClass);

            foreach (var factoryClass in interfaces)
            {
                config.SelfBind(factoryClass);
            }
        }

        static void BindDynamicModelSaveFactories(DiContainerConfig config, string[] assemblyPrefixes)
        {
            var factoryType = typeof(ModelFactoryBase<,>);
            var interfaces = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => assemblyPrefixes.Any(x => a.FullName.Contains(x)))
                .SelectMany(a => a.GetTypes())
                .Where(type => type != factoryType)
                .Where(type => typeof(IModelFactoryBase).IsAssignableFrom(type))
                .Where(type => type.IsClass);

            foreach (var factoryClass in interfaces)
            {
                config.SelfBind(factoryClass);
            }
        }
    }
}