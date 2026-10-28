
using JG.Logic.Entities.Interfaces.Services.Saves;

namespace JG.Logic.Entities.Services.Saves
{
    public class GameSavePlayerPrefsKeyService : IGameSavePlayerPrefsKeyService
    {
        public string GetKey()
        {
            return "GameSave";
        }
    }
}
