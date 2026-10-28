using JG.Data.Entities.Interfaces.Entities.Common;

namespace ZW.Data.Interfaces.Entities.Settings
{
    public interface IZooSettingsEntity : IEntity, ISettingsEntity
    {
        float SpawnIntervalMin { get; }
        float SpawnIntervalMax { get; }
        float ScreenPadding { get; }
        float TastyLabelDuration { get; }
    }
}
