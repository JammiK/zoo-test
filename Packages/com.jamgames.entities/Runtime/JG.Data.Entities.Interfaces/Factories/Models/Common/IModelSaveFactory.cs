using JG.Data.Entities.Interfaces.Models.Common;

namespace JG.Data.Entities.Interfaces.Factories.Models.Common
{
    public interface IModelSaveFactory<T, TS> : IModelFactory<T>
        where T : IModel
    {
        TS CreateSaveInternal(T model);
        T Create(TS modelSave);
    }
}