using JG.Data.Entities.Interfaces.Models.Common;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Types.Animals;
using ZW.Data.Models.Types;

namespace ZW.Data.Interfaces.Models.Animals
{
    public interface IAnimalModel : IModel
    {
        int Uid { get; set; }
        IAnimalEntity Entity { get; set; }
        AnimalState State { get; set; }
        PlanarVector Position { get; set; }
        PlanarVector Direction { get; set; }
        float MoveCooldown { get; set; }
        bool KeepDirection { get; set; }
    }
}
