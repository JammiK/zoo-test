using JG.Data.Generators.Systems;
using UnityEditor;

namespace JG.Data.Generators.Buttons
{
    public class GenerateEntitiesButton
    {
        static EntityGeneratorSystem _entityGeneratorSystem;
        static ContextGeneratorSystem _contextGeneratorSystem;
        static ModelGeneratorSystem _modelGeneratorSystem;

        static GenerateEntitiesButton()
        {
            _entityGeneratorSystem = new EntityGeneratorSystem();
            _contextGeneratorSystem = new ContextGeneratorSystem();
            _modelGeneratorSystem = new ModelGeneratorSystem();
        }

        [MenuItem("[JG]/Generator/Generate ALL")]
        static void CreateAll()
        {
            _entityGeneratorSystem.GenerateEntities();
            _contextGeneratorSystem.GenerateContexts();
            _modelGeneratorSystem.GenerateModels();
        }

        
        [MenuItem("[JG]/Generator/Generate Entities")]
        static void CreateEntities()
        {
            _entityGeneratorSystem.GenerateEntities();
        }
        
        [MenuItem("[JG]/Generator/Generate Contexts")]
        static void CreateContexts()
        {
            _contextGeneratorSystem.GenerateContexts();
        }

        [MenuItem("[JG]/Generator/Generate Models")]
        static void CreateModels()
        {
            _modelGeneratorSystem.GenerateModels();
        }
    }
}