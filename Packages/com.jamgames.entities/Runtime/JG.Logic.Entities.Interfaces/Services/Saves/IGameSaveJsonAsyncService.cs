using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace JG.Logic.Entities.Interfaces.Services.Saves
{
    public interface IGameSaveJsonAsyncService
    {
        UniTask<bool> CheckGameSaveExists();
        UniTask<List<object>> LoadGameSaveJson();
        UniTask SaveGameContext(List<object> gameSaves);
    }
}