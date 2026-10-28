using System;
using JG.Data.Entities.Interfaces.Models.Common;

namespace JG.Data.Entities.Interfaces.Factories.Models.Common
{
    public interface IModelFactoryBase
    {
        Type GetModelType { get; }
        bool CheckAvailableSave(object modelSave);
        IModel CreateModel();
        object CreateSave(IModel model);
        IModel CreateModel(object modelSave);
    }
}