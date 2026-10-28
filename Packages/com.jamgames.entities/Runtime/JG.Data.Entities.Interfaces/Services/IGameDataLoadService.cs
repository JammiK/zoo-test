using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Entities.Common;

namespace JG.Data.Entities.Interfaces.Services
{
    public interface IGameDataLoadService
    {
        IDictionary<string, T> LoadDictionary<T>(string fileName) where T : IEntity;
        List<string> LoadList(string fileName, bool encrypt);
        List<string> LoadSymbols(string fileName);
        List<string> LoadWords(string fileName);
        T LoadObject<T>(string fileName) where T : IEntity;
    }
}