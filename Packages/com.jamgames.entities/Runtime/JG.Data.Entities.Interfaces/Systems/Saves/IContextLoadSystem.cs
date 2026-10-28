using JG.Data.Entities.Interfaces.Providers.Contexts;

namespace JG.Data.Entities.Interfaces.Systems.Saves
{
    public interface IContextLoadSystem
    {
        void LoadContexts(IContextProvider contextProvider);
    }
}