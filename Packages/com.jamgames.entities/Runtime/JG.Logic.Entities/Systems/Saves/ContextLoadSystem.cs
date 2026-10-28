

using JG.Data.Entities.Interfaces.Factories.Contexts.Common;
using JG.Data.Entities.Interfaces.Providers.Contexts;
using JG.Data.Entities.Interfaces.Systems.Saves;
using JG.Logic.Entities.Interfaces.Repositories.Saves;
using JG.Logic.Entities.Interfaces.Streams.Saves;

namespace JG.Logic.Entities.Systems.Saves
{
    public class ContextLoadSystem : IContextLoadSystem
    {
        readonly IContextFactory[] _contextFactories;
        readonly ILoadContextsStream _loadContextsStream;
        readonly IGameSaveRepository _gameSaveRepository;
        IContextProvider _contextProvider;

        public ContextLoadSystem(IContextFactory[] contextFactories,
            ILoadContextsStream loadContextsStream,
            IGameSaveRepository gameSaveRepository)
        {
            _contextFactories = contextFactories;
            _loadContextsStream = loadContextsStream;
            _gameSaveRepository = gameSaveRepository;
        }

        public void LoadContexts(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
            CreateContexts();

            _loadContextsStream.OnNext(true);
        }
        
        void CreateContexts()
        {
            if (_gameSaveRepository.SaveExists())
            {
                var contextSaves = _gameSaveRepository.GetGameSave();

                if (contextSaves != null)
                {
                    foreach (var contextFactory in _contextFactories)
                    {
                        var contextFound = false;
                        if (!contextFactory.IgnoreSave)
                        {
                            foreach (var contextSave in contextSaves)
                            {
                                if (contextFactory.CheckAvailableSave(contextSave))
                                {
                                    contextFound = true;
                                    var context = contextFactory.CreateContext(contextSave);
                                    _contextProvider.AddContext(contextFactory, context);
                                    break;
                                }
                            }
                        }

                        if (contextFound == false)
                        {
                            // Log.Info($"Failed to find context for factory {contextFactory.GetContextType}, creating new");
                            CreateContext(contextFactory);
                        }
                    }

                    return;
                }

                // Log.Error("Context saves is null, creating new");
            }

            foreach (var contextFactory in _contextFactories)
            {
                CreateContext(contextFactory);
            }
        }

        void CreateContext(IContextFactory contextFactory)
        {
            var context = contextFactory.CreateContext();
            _contextProvider.AddContext(contextFactory, context);
        }
    }
}