using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JG.Data.Entities.Interfaces.Entities.Common;

namespace JG.Data.Entities.Interfaces.Repositories.Common
{
    public interface IEntityRepository<T> where T : IEntity
    {
        ICollection<T> GetAll();
        [CanBeNull] T Get(string entityId);
        [CanBeNull] T GetFirst(Predicate<T> predicate);
        IEnumerable<T> Get(Predicate<T> predicate);
        bool IsExist(string entityId);

        [CanBeNull]
        T GetRandom();
    }
}