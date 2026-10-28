using JG.Data.Entities.Interfaces.Attributes;
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
        [DefaultValueSnippet("1.5f")] float TastyLabelRiseDistance { get; }
        [PrefabField] string TastyLabelPrefab { get; }
        [DefaultValueSnippet("5f")] float AutosaveInterval { get; }
        [DefaultValueSnippet("60")] int MaxAnimals { get; }
    }
}
