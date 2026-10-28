using System.Collections.Generic;
using JG.Logic.Entities.Interfaces.Repositories.Saves;
using JG.Logic.Entities.Interfaces.Services.Saves;

namespace JG.Logic.Entities.Repositories.Saves
{
    public class GameSaveRepository : IGameSaveRepository
    {
        readonly IGameSaveJsonService _gameSaveJsonService;

        public GameSaveRepository(IGameSaveJsonService gameSaveJsonService)
        {
            _gameSaveJsonService = gameSaveJsonService;
        }

        public bool SaveExists()
        {
            return _gameSaveJsonService.CheckGameSaveExists();
        }

        public List<object> GetGameSave()
        {
            return _gameSaveJsonService.LoadGameSaveJson();
        }
    }
}
