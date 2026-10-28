using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.JG.Extensions;
using JG.Data.Entities.Editor.Common;
using JG.Data.Entities.Editor.Types;
using JG.Data.Entities.Interfaces.Attributes;
using JG.Data.Entities.Interfaces.Entities.Common;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace JG.Data.Entities.Editor.Buttons
{
    public class ParseEntitiesEditorButton
    {
        static readonly string[] DatabaseFolders = {"Assets/GameData"};
        static readonly string[] TestDatabaseFolders = {"Assets/GameData"};

        static string[] RootDatabaseFolders = {"Assets/GameData"};
        static string RootSaveFolder = "Assets/Resources/GameData/Runtime/";

        [MenuItem("[JG]/Game Data/Create Develop Game Data")]
        public static void CreateGameData()
        {
            RootSaveFolder = "Assets/Resources/GameData/Runtime/";
            RootDatabaseFolders = DatabaseFolders;
            LoadAll(GameDataType.Develop);
        }

        [MenuItem("[JG]/Game Data/Create Test Game Data")]
        static void CreateTestGameData()
        {
            RootSaveFolder = "Assets/Resources/GameData/Runtime/Tests/0/";
            RootDatabaseFolders = TestDatabaseFolders;
            LoadAll(GameDataType.Test);
        }

        [MenuItem("[JG]/Game Data/Create Production Game Data")]
        static void CreateProductionGameData()
        {
            RootSaveFolder = "Assets/Resources/GameData/Runtime/";
            RootDatabaseFolders = TestDatabaseFolders;
            LoadAll(GameDataType.Release);
        }

        static void LoadAll(GameDataType gameDataType)
        {
            var entityType = typeof(IEntity);
            var editorEntityType = typeof(IEditorEntity<>);
            var assembly = AppDomain.CurrentDomain.GetAssemblies();
            var entityInterfaces = assembly.
                Where(a => a.FullName[2] == '.' || a.FullName[3] == '.')
                .SelectMany(it => it.GetTypes())
                .Where(type => type != entityType)
                .Where(type => entityType.IsAssignableFrom(type) && !editorEntityType.IsAssignableFrom(type))
                .Where(type => type.IsInterface)
                .Where(type => !type.IsAbstractEntity());
            foreach (var entityInterface in entityInterfaces)
            {
                var genericType = typeof(EditorEntityBase<>).MakeGenericType(entityInterface);
                var editorEntities = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName[2] == '.' || a.FullName[3] == '.')
                    .SelectMany(it => it.GetTypes())
                    .Where(it => it.IsSubclassOf(genericType))
                    .Where(type => type != typeof(EditorEntityBase<>))
                    .Where(type => entityInterface.IsAssignableFrom(type))
                    .Where(type => type.IsClass)
                    .ToArray();
                
                if (editorEntities.Length != 1)
                {
                    throw new Exception($"Wrong amount of EditorEntities for {entityInterface}");
                }

                var isSingleEntity = false;
                var customFileName = string.Empty;
                foreach (var customAttribute in entityInterface.GetCustomAttributes(true))
                {
                    if (customAttribute is SingleEntityAttribute)
                    {
                        isSingleEntity = true;
                    }

                    if (customAttribute is CustomEntityFileNameAttribute customEntityFileName)
                    {
                        customFileName = customEntityFileName.FileName;
                    }
                }

                if (string.IsNullOrEmpty(customFileName))
                {
                    var fileName = entityInterface.Name.Split('.').Last().Substring(1).Replace("Entity", string.Empty);
                    if (!isSingleEntity)
                    {
                        fileName = $"{fileName}s";
                    }

                    customFileName = fileName;
                }
                
                // if (!isSingleEntity)
                {
                    var method = typeof(ParseEntitiesEditorButton).GetMethod(nameof(LoadEditorEntities), BindingFlags.Static | BindingFlags.Public);
                    var generic = method.MakeGenericMethod(editorEntities[0], entityInterface);
                    generic.Invoke(null, new object[] {customFileName, null});
                }
            }
        }

        public static void LoadEditorEntities<T, TU>(string entityName, Predicate<T> predicate = null)
            where T : ScriptableObject, IEditorEntity<TU>
            where TU : IEntity
        {
            var entities = GetFilteredEntityArray<T, TU>(entityName, predicate);

            Save(entities, entityName);
            Debug.Log($"Load {entityName}: {entities.Length}");
        }

        static void LoadEditorEntity<T, TU>(string entityName, Predicate<T> predicate = null)
            where T : ScriptableObject, IEditorEntity<TU>
            where TU : IEntity
        {
            var entity = GetFilteredEntityArray<T, TU>(entityName, predicate, true).FirstOrDefault();
            if (entity == null)
            {
                return;
            }

            Save(entity, entityName);
            Debug.Log($"Load {entityName}");
        }

        static TU[] GetFilteredEntityArray<T, TU>(string entityName, Predicate<T> predicate = null, bool ignoreId = false)
            where T : ScriptableObject, IEditorEntity<TU>
            where TU : IEntity
        {
            var editorEntities = LoadAssetsFromPath<T, TU>(entityName);

            if (predicate != null)
            {
                editorEntities = editorEntities.Where(model => predicate(model));
            }

            if (!ignoreId)
            {
                foreach (var entities in editorEntities.GroupBy(entity => entity.Id))
                {
                    if (entities.Count() > 1)
                    {
                        throw new DataException($"{typeof(T)} has multiply entities with the same id {entities.Key}");
                    }
                }
            }

            return ToFilteredEntity<T, TU>(editorEntities).ToArray();
        }


        static IEnumerable<T> LoadAssetsFromPath<T, TU>(string entityName)
            where T : ScriptableObject, IEditorEntity<TU>
            where TU : IEntity
        {
            var last = typeof(T).FullName.Split('.').Last();
            return AssetDatabase.FindAssets($"t:{last}", RootDatabaseFolders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>);
        }

        static IEnumerable<TU> ToFilteredEntity<T, TU>(IEnumerable<T> editorEntities)
            where T : ScriptableObject, IEditorEntity<TU>
            where TU : IEntity
        {
            return editorEntities.Where(model => !model.Disabled)
                .Select(editorModel => editorModel.ToEntity());
        }

        static void Save(object gameData, string fileName)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            var sb = new StringBuilder();
            var json = JsonConvert.SerializeObject(gameData, jsonSerializerSettings);
            json = EncryptionExtension.EncryptDecrypt(sb, json);

            if (!Directory.Exists(RootSaveFolder))
            {
                Directory.CreateDirectory(RootSaveFolder);
            }
            
            var writer = new StreamWriter($"{RootSaveFolder}{fileName}.json", false);
            writer.Write(json);
            writer.Close();

            AssetDatabase.Refresh();
        }
    }
}