using JG.Data.Entities.Interfaces.Entities.Common;
using ZW.Data.Interfaces.Entities.Levels;

namespace ZW.Data.Interfaces.Entities.Settings
{
    public interface IZooSettingsEntity : IEntity, ISettingsEntity
    {
        ILevelEntity Level { get; }
        float SpawnIntervalMin { get; }
        float SpawnIntervalMax { get; }
        float ScreenPadding { get; }
        float TastyLabelDuration { get; }
    }
}
