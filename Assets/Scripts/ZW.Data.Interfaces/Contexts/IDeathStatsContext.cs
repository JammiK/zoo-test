using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Contexts.Common;

namespace ZW.Data.Interfaces.Contexts
{
    public interface IDeathStatsContext : IContext
    {
        Dictionary<string, int> DeathsByFoodChain { get; set; }
    }
}
