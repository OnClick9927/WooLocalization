/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace WooLocalization
{
    public abstract class ActorAsset<Value> : ScriptableObject, IActorContext<Value>
    {
        [SerializeField]
        private List<string> keys = new List<string>();
        [SerializeField]
        private SerializableDictionary<string, SerializableDictionary<string, Value>> map = new SerializableDictionary<string, SerializableDictionary<string, Value>>();

        [System.NonSerialized] private List<string> localizationTypes;
        [System.NonSerialized] private IReadOnlyList<string> readOnlyLocalizationTypes;
        [System.NonSerialized] private int localizationTypesVersion = -1;
        [System.NonSerialized] private string cachedLanguage;
        [System.NonSerialized] private SerializableDictionary<string, Value> cachedLanguageMap;
        [System.NonSerialized] private int cachedMapVersion = -1;

        public List<string> GetLocalizationKeys() => keys;
        public List<string> GetLocalizationTypes()
        {
            RefreshLocalizationTypes();
            return new List<string>(localizationTypes);
        }

        public IReadOnlyList<string> GetLocalizationTypesReadOnly()
        {
            RefreshLocalizationTypes();
            if (readOnlyLocalizationTypes == null)
                readOnlyLocalizationTypes = localizationTypes.AsReadOnly();
            return readOnlyLocalizationTypes;
        }

        private void RefreshLocalizationTypes()
        {
            if (localizationTypes == null)
                localizationTypes = new List<string>(map.Count);
            if (localizationTypesVersion != map.Version)
            {
                map.CopyKeysTo(localizationTypes);
                localizationTypesVersion = map.Version;
            }
        }

        public Value GetLocalization(string language, string key)
        {
            if (cachedMapVersion != map.Version || !string.Equals(cachedLanguage, language, System.StringComparison.Ordinal))
            {
                map.TryGetValue(language, out cachedLanguageMap);
                cachedLanguage = language;
                cachedMapVersion = map.Version;
            }

            Value value;
            if (cachedLanguageMap != null && cachedLanguageMap.TryGetValue(key, out value))
                return value;
            return default;
        }
        public void Merge(IActorContext<Value> context,string language)
        {
            var keys = context.GetLocalizationKeys();
            var knownKeys = new HashSet<string>(this.keys);
            for (var j = 0; j < keys.Count; j++)
            {
                var key = keys[j];
                var value = context.GetLocalization(language, key);
                AddPair(language, key, value, knownKeys);
            }
        
        }
        public void Merge(IActorContext<Value> context)
        {
            var actorContext = context as ActorAsset<Value>;
            IReadOnlyList<string> types = actorContext == null
                ? context.GetLocalizationTypes()
                : actorContext.GetLocalizationTypesReadOnly();
            var keys = context.GetLocalizationKeys();
            var knownKeys = new HashSet<string>(this.keys);
            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];
                for (var j = 0; j < keys.Count; j++)
                {
                    var key = keys[j];
                    var value = context.GetLocalization(type, key);
                    AddPair(type, key, value, knownKeys);
                }
            }
        }



        public void AddPair(string language, string key, Value value)
        {
            AddPair(language, key, value, null);
        }

        private void AddPair(string language, string key, Value value, HashSet<string> knownKeys)
        {
            if (string.IsNullOrEmpty(language) || string.IsNullOrEmpty(key)) return;
            var isNewKey = knownKeys == null ? !keys.Contains(key) : knownKeys.Add(key);
            if (isNewKey)
                keys.Add(key);

            SerializableDictionary<string, Value> dic;
            if (!map.TryGetValue(language, out dic))
            {
                dic = new SerializableDictionary<string, Value>();
                map.Add(language, dic);
            }
            dic[key] = value;
        }
        public void ClearLanguage(string language)
        {
            SerializableDictionary<string, Value> result;
            if (map.TryGetValue(language, out result))
                result.Clear();
        }

        public void ClearKeys(IList<string> keys)
        {
            foreach (var language in map)
            {
                var values = language.Value;
                for (var i = keys.Count - 1; i >= 0; i--)
                    values.Remove(keys[i]);
            }

            for (int i = keys.Count - 1; i >= 0; i--)
            {
                var key = keys[i];
                this.keys.Remove(key);
            }
        }

        public void Clear()
        {
            map.Clear();
            keys.Clear();
            cachedLanguage = null;
            cachedLanguageMap = null;
            cachedMapVersion = map.Version;
        }

        public string FindKey(string language, Value val)
        {
            if (map.TryGetValue(language, out var key_value))
            {
                var comparer = EqualityComparer<Value>.Default;
                foreach (var item in key_value)
                {
                    if (comparer.Equals(item.Value, val))
                        return item.Key;
                }
            }
            return string.Empty;
        }
        internal SerializableDictionary<string, Value> AddLanguage(string language)
        {
            SerializableDictionary<string, Value> dic;
            if (!map.TryGetValue(language, out dic))
            {
                dic = new SerializableDictionary<string, Value>();
                map.Add(language, dic);
            }
            return dic;
        }

    }
}
