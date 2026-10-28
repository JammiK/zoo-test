using JG.Data.Entities.Interfaces.Factories.Models.Common;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Factories.Animals;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Interfaces.Types.Animals;
using ZW.Data.Models.Types;

namespace ZW.Data.Factories.Animals
{
    public class AnimalFactory : IAnimalFactory
    {
        private readonly IModelFactory<IAnimalModel> _modelFactory;

        public AnimalFactory(IModelFactory<IAnimalModel> modelFactory)
        {
            _modelFactory = modelFactory;
        }

        public IAnimalModel Create(int uid, IAnimalEntity entity, PlanarVector position, PlanarVector direction)
        {
            var animal = _modelFactory.Create();
            animal.Uid = uid;
            animal.Entity = entity;
            animal.State = AnimalState.Alive;
            animal.Position = position;
            animal.Direction = direction;
            return animal;
        }
    }
}
