using System;
using JG.Data.Entities.Interfaces.Factories.Models.Common;
using JG.Data.Entities.Interfaces.Models.Common;

namespace JG.Data.Entities.Models
{
    public abstract class ModelFactoryBase<T, TS> : IModelFactoryBase where T : IModel
    {
        public Type GetModelType => typeof(T);
        public bool CheckAvailableSave(object modelSave)
        {
            return modelSave is TS;
        }

        public IModel CreateModel()
        {
            return Create();
        }

        public object CreateSave(IModel model)
        {
            if (model is T tModel)
            {
                return CreateSaveInternal(tModel);
            }
            
            throw new Exception($"Unable to cast {model.GetType()} to {typeof(T)}");
        }

        public IModel CreateModel(object modelSave)
        {
            if (modelSave is TS tsModelSave)
            {
                return Create(tsModelSave);
            }
            
            throw new Exception($"Unable to cast {modelSave.GetType()} to {typeof(T)}");
        }
        
        public abstract T Create();
        public abstract TS CreateSaveInternal(T tContext);
        public abstract T Create(TS contextSave);
    }
}