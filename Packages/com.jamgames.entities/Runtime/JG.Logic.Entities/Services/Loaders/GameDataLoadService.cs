using System;
using System.Collections.Generic;
using System.Linq;
using Core.JG.Extensions;
using Core.JG.Logic.Core.Interfaces.Services.Loaders;
using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Entities.Interfaces.Services;
using Newtonsoft.Json;

namespace JG.Logic.Entities.Services.Loaders
{
    public class GameDataLoadService : IGameDataLoadService
    {
        const string GameDataPathFormat = "GameData/Runtime/{0}";

        readonly ITextLoadService _textLoadService;

        public GameDataLoadService(ITextLoadService textLoadService)
        {
            _textLoadService = textLoadService;
        }

        public IDictionary<string, T> LoadDictionary<T>(string fileName) where T : IEntity
        {
            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            var json = _textLoadService.Load(string.Format(GameDataPathFormat, fileName), true, true);
            if (string.IsNullOrEmpty(json))
            {
                return new Dictionary<string, T>();
            }
            
            var entities = JsonConvert.DeserializeObject<T[]>(json, serializerSettings);

            var groupedEntities = entities.GroupBy(x => x.Id);
            foreach (var groupedEntity in groupedEntities)
            {
                if (groupedEntity.Count() > 1)
                {
                    // Log.Error($"Entities {typeof(T)} has multiply objects with the same id {groupedEntity.Key}");
                }
            }

            var distinctEntities = groupedEntities.Select(y => y.First()).ToList();
            var dict = distinctEntities.ToDictionary(entity => entity.Id, entity => entity);
            return dict;
        }

        public List<string> LoadList(string fileName, bool encrypt)
        {
            var list = _textLoadService.Load(fileName, encrypt, true);
            if (string.IsNullOrEmpty(list))
            {
                return new List<string>();
            }
            
            string[] lst = list.Split('\n');
            return lst.ToList();
        }

        public List<string> LoadSymbols(string fileName)
        {
            var list = _textLoadService.Load(fileName, false);

            if (string.IsNullOrEmpty(list))
                return new List<string>();

            return SymbolExtensions.ParseSymbols(list);
        }

        public List<string> LoadWords(string fileName)
        {
            var text = _textLoadService.Load(fileName, false);

            if (string.IsNullOrEmpty(text))
                return new List<string>();

            List<string> words = new List<string>(text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
            // убираются дубликаты
            words = words.Distinct().ToList();
            // сортировка слов по длине в порядке убывания
            words.Sort((a, b) =>
            {
                if (a.Length < b.Length)
                    return 1;
                if (a.Length > b.Length)
                    return -1;
                return 0;
            });

            return words;
        }

        public T LoadObject<T>(string fileName) where T : IEntity
        {
            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            var json = _textLoadService.Load(string.Format(GameDataPathFormat, fileName), true);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }
            
            return JsonConvert.DeserializeObject<T>(json, serializerSettings);
        }
    }
}