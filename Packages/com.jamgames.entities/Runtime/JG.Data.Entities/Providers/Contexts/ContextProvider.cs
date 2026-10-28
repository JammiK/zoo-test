using System;
using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Contexts.Common;
using JG.Data.Entities.Interfaces.Factories.Contexts.Common;
using JG.Data.Entities.Interfaces.Providers.Contexts;
using JG.Data.Entities.Interfaces.Systems.Saves;

namespace JG.Data.Entities.Providers.Contexts
{
    public class ContextProvider : IContextProvider
    {
        public ContextProvider(IContextLoadSystem contextLoadSystem)
        {
            contextLoadSystem.LoadContexts(this);
        }

        public Dictionary<IContext, IContextFactory> Contexts { get; } = new Dictionary<IContext, IContextFactory>();
        readonly Dictionary<Type, IContext> _contextMap = new Dictionary<Type, IContext>();

        public void AddContext(IContextFactory contextFactory, IContext context)
        {
            _contextMap[contextFactory.GetContextType] = context;
            Contexts.Add(context, contextFactory);
        }

        public T Get<T>() where T : IContext
        {
            if (_contextMap.TryGetValue(typeof(T), out var context))
            {
                return (T)context;
            }

            throw new Exception($"Unable to get context with type {typeof(T)}");
        }
    }
}
