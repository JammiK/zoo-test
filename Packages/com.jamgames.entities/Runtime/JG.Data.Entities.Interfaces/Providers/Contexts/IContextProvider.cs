using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Contexts.Common;
using JG.Data.Entities.Interfaces.Factories.Contexts.Common;

namespace JG.Data.Entities.Interfaces.Providers.Contexts
{
    public interface IContextProvider
    {
        Dictionary<IContext, IContextFactory> Contexts { get; }
        T Get<T>() where T : IContext;
        void AddContext(IContextFactory contextFactory, IContext context);
    }
}
