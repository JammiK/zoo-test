using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JG.Data.Entities.Interfaces.Attributes;
using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Entities.Interfaces.Providers.GameData;
using JG.Data.Entities.Interfaces.Services;

namespace JG.Data.Entities.Providers.GameData
{
    public class GameDataProvider : IGameDataProvider
    {
        Dictionary<Type, object> _dictionary = new Dictionary<Type, object>();
        readonly Dictionary<Type, object> _settings = new Dictionary<Type, object>();
        
        const string EntityName = "IEntity";
        public GameDataProvider(IGameDataLoadService gameDataLoadService)
        {
            var entityType = typeof(IEntity);
            var entityInterfaces = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName[2] == '.' || a.FullName[3] == '.')
                .SelectMany(a => a.GetTypes())
                .Where(type => type != entityType)
                .Where(type => entityType.IsAssignableFrom(type) && !typeof(IEditorEntity<>).IsAssignableFrom(type))
                .Where(type => type.IsInterface)
                .Where(type => !type.IsAbstractEntity())
                .ToArray();
            
            foreach (var entityInterface in entityInterfaces)
            {
                string fileName = null;
                var isSingleEntity = false;
                foreach (var customAttribute in entityInterface.GetCustomAttributes())
                {
                    if (customAttribute is CustomEntityFileNameAttribute customEntityFileNameAttribute)
                    {
                        fileName = customEntityFileNameAttribute.FileName;
                    }

                    if (customAttribute is SingleEntityAttribute)
                    {
                        isSingleEntity = true;
                    }
                }

                if (string.IsNullOrEmpty(fileName))
                {
                    if (entityInterface.Name.Contains(EntityName.Substring(1))) 
                    {
                        fileName = entityInterface.Name.Substring(1, entityInterface.Name.Length - EntityName.Length);
                    } 
                    else
                    {
                        fileName = entityInterface.Name.Substring(1);
                    }
                    
                    if (!isSingleEntity)
                    {
                        fileName = $"{fileName}s";
                    }
                }

                var methodInfo = gameDataLoadService.GetType()
                    .GetMethod("LoadDictionary");
                if (methodInfo == null)
                {
                    throw new Exception($"Unable to find method 'LoadDictionary' for gameDataLoadService");
                }

                var loadResult = methodInfo.MakeGenericMethod(entityInterface)
                        .Invoke(gameDataLoadService, new object[] {fileName});
                _dictionary.Add(entityInterface, loadResult);
            }
        }

        public IDictionary<string, T> GetEntities<T>() where T : IEntity
        {
            if (!_dictionary.TryGetValue(typeof(T), out var abstractObj))
            {
                throw new Exception($"Unable to find Entity Map for {typeof(T)}");
            }

            if (abstractObj is IDictionary<string, T> entityMap)
            {
                return entityMap;
            }
            
            throw new Exception($"Unable to cast Entity Map for {typeof(T)}");
        }

        public T GetSettings<T>() where T : IEntity
        {
            if (_settings.TryGetValue(typeof(T), out var cached))
            {
                return (T)cached;
            }

            if (!_dictionary.TryGetValue(typeof(T), out var abstractObj))
            {
                throw new Exception($"Unable to find Entity Map for {typeof(T)}");
            }

            if (abstractObj is IDictionary<string, T> entityMap)
            {
                var settings = entityMap.Values.ElementAt(0);
                _settings[typeof(T)] = settings;
                return settings;
            }
            
            throw new Exception($"Unable to cast Entity Map for {typeof(T)}");
        }
    }
}