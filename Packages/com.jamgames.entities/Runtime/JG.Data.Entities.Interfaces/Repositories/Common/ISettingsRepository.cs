using JG.Data.Entities.Interfaces.Entities.Common;

namespace JG.Data.Entities.Interfaces.Repositories.Common
{
    public interface ISettingsRepository
    {
        T Get<T>() where T : ISettingsEntity, IEntity;
    }
}