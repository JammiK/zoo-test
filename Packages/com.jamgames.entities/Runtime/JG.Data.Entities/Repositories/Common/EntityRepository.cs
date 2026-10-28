using System;
using System.Collections.Generic;
using System.Linq;
using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Entities.Interfaces.Providers.GameData;
using JG.Data.Entities.Interfaces.Repositories.Common;

namespace JG.Data.Entities.Repositories.Common
{
    public class EntityRepository<T> : IEntityRepository<T> where T : IEntity
    {
        IDictionary<string, T> _map;
        T[] _valuesCache;
        
        
        public EntityRepository(IGameDataProvider gameDataProvider)
        {
            _map = gameDataProvider.GetEntities<T>();
            _valuesCache = _map.Values.ToArray();
        }

        public ICollection<T> GetAll()
        {
            return _map.Values;
        }

        public T Get(string entityId)
        {
            if (entityId != null && _map != null && _map.TryGetValue(entityId, out var entity))
            {
                return entity;
            }

            return default;
        }

        public T GetFirst(Predicate<T> predicate)
        {
            for (var index = 0; index < _valuesCache.Length; index++)
            {
                var entity = _valuesCache[index];
                if (predicate(entity))
                {
                    return entity;
                }
            }

            return default;
        }

        public IEnumerable<T> Get(Predicate<T> predicate)
        {
            for (var index = 0; index < _valuesCache.Length; index++)
            {
                var entity = _valuesCache[index];
                if (predicate(entity))
                {
                    yield return entity;
                }
            }
        }

        public bool IsExist(string entityId)
        {
            return _map.ContainsKey(entityId);
        }

        public T GetRandom()
        {
            if (_valuesCache.Length == 0)
            {
                return default;
            }

            var index = UnityEngine.Random.Range(0, _valuesCache.Length);
            return _valuesCache[index];
        }
    }
}
