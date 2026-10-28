using JG.Data.Entities.Interfaces.Contexts.Common;

namespace JG.Data.Entities.Interfaces.Factories.Contexts.Common
{
    public interface IContextSaveFactory<T,TS> where T : IContext
    {
    }
}