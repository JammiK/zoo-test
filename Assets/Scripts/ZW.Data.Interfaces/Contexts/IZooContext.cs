using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Attributes;
using JG.Data.Entities.Interfaces.Contexts.Common;
using ZW.Data.Interfaces.Models.Animals;

namespace ZW.Data.Interfaces.Contexts
{
    public interface IZooContext : IContext
    {
        List<IAnimalModel> Animals { get; set; }
        int NextUid { get; set; }
        float SpawnCooldown { get; set; }
        [IgnoreSave] Dictionary<int, IAnimalModel> AnimalsByUid { get; set; }
    }
}
