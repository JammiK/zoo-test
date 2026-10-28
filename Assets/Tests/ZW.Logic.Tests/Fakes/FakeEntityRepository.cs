using System;
using System.Collections.Generic;
using System.Linq;
using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Entities.Interfaces.Repositories.Common;

namespace ZW.Logic.Tests.Fakes
{
    public class FakeEntityRepository<T> : IEntityRepository<T> where T : IEntity
    {
        private readonly List<T> _entities;

        public FakeEntityRepository(params T[] entities)
        {
            _entities = entities.ToList();
        }

        public ICollection<T> GetAll() => _entities;
        public T Get(string entityId) => _entities.FirstOrDefault(entity => entity.Id == entityId);
        public T GetFirst(Predicate<T> predicate) => _entities.FirstOrDefault(entity => predicate(entity));
        public IEnumerable<T> Get(Predicate<T> predicate) => _entities.Where(entity => predicate(entity));
        public bool IsExist(string entityId) => _entities.Any(entity => entity.Id == entityId);
        public T GetRandom() => _entities.FirstOrDefault();
    }
}
