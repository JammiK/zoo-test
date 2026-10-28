
using System.Collections.Generic;

namespace JG.Logic.Entities.Interfaces.Services.Saves
{
    public interface IGameSaveJsonService
    {
        bool CheckGameSaveExists();
        List<object> LoadGameSaveJson();
        void SaveGameContext(List<object> gameSaves);
    }
}
