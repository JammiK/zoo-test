using JG.Data.Entities.Interfaces.Models.Common;

namespace JG.Data.Entities.Interfaces.Factories.Models.Common
{
    public interface IModelFactory<out TModel> where TModel : IModel
    {
        TModel Create();
    }
}