/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WooLocalization
{
    public abstract class ActorAsset<Value> : ScriptableObject, IActorContext<Value>
    {

        [SerializeField]
        private List<string> keys = new List<string>();
        [SerializeField]
        private SerializableDictionary<string, SerializableDictionary<string, Value>> map = new SerializableDictionary<string, SerializableDictionary<string, Value>>();

        public List<string> GetLocalizationKeys() => keys;
        public List<string> GetLocalizationTypes() => map.Keys.ToList();
        public Value GetLocalization(string language, string key)
        {
            if (map.TryGetValue(language, out var dic))
            {
                if (dic.TryGetValue(key, out var value))
                {
                    return value;
                }
            }
            return default;

        }
        public void Merge(IActorContext<Value> context)
        {
            var types = context.GetLocalizationTypes();
            var keys = context.GetLocalizationKeys();
            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];
                for (var j = 0; j < keys.Count; j++)
                {
                    var key = keys[j];
                    var value = context.GetLocalization(type, key);
                    AddPair(type, key, value);
                }
            }
        }



        public void AddPair(string language, string key, Value value)
        {
            if (!keys.Contains(key))
                keys.Add(key);
            SerializableDictionary<string, Value> dic = null;
            if (!map.TryGetValue(language, out dic))
            {
                dic = new SerializableDictionary<string, Value>();
                map.Add(language, dic);
            }
            dic[key] = value;
        }
        public void ClearLanguage(string language) => map.Remove(language);
        public void ClearKeys(IList<string> keys)
        {
            var lanTypes = this.map.Keys.ToList();
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                var key = keys[i];

                for (int j = 0; j < lanTypes.Count; j++)
                {
                    var type = lanTypes[j];
                    map[type].Remove(key);
                }

                this.keys.Remove(key);
            }
        }
        public void Clear()
        {
            map.Clear();
            keys.Clear();
        }
        public string FindKey(string language, Value val)
        {
            if (map.TryGetValue(language, out var key_value))
            {
                foreach (var item in key_value)
                {
                    if (item.Value.Equals(val))
                    {
                        return item.Key;
                    }
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
