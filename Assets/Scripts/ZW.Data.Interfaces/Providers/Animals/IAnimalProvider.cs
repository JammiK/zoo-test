using System.Collections.Generic;
using JetBrains.Annotations;
using ZW.Data.Interfaces.Models.Animals;

namespace ZW.Data.Interfaces.Providers.Animals
{
    public interface IAnimalProvider
    {
        IReadOnlyList<IAnimalModel> GetAll();
        IEnumerable<IAnimalModel> GetAlive();
        [CanBeNull] IAnimalModel Get(int uid);
    }
}
