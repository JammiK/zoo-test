using System;
using JG.Data.Entities.Interfaces.Attributes;
using JG.Data.Entities.Interfaces.Contexts.Common;

namespace JG.Data.Entities.Interfaces.Factories.Contexts.Common
{
    public interface IContextFactory
    {
        bool IgnoreSave { get; }
        ContextLifeScope LifeScope { get; }
        Type GetContextType { get; }
        Type GetContextSaveType { get; }
        bool CheckAvailableSave(object contextSave);
        IContext CreateContext();
        object CreateSave(IContext context);
        IContext CreateContext(object contextSave);
    }
}