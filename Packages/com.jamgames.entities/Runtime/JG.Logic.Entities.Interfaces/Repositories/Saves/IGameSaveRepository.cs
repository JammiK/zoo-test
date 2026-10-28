
using System.Collections.Generic;

namespace JG.Logic.Entities.Interfaces.Repositories.Saves
{
    public interface IGameSaveRepository
    {
        bool SaveExists();
        List<object> GetGameSave();
    }
}
