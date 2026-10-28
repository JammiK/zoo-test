using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Entities.Common;

namespace JG.Data.Entities.Interfaces.Providers.GameData
{
    public interface IGameDataProvider
    {
        IDictionary<string, T> GetEntities<T>() where T : IEntity;
        T GetSettings<T>() where T : IEntity;
    }
}