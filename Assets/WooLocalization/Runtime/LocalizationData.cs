﻿/*********************************************************************************
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
    [CreateAssetMenu]
    public class LocalizationData : ScriptableObject, ILocalizationContext
    {
        [UnityEngine.SerializeField]
        private SerializableDictionary<string, SerializableDictionary<string, string>> map
            = new SerializableDictionary<string, SerializableDictionary<string, string>>();


        [UnityEngine.SerializeField]
        private List<string> keys = new List<string>();

        public string GetLocalization(string localizationType, string key)
        {
            if (!map.ContainsKey(localizationType)) return string.Empty;
            if (!map[localizationType].ContainsKey(key)) return string.Empty;
            return map[localizationType][key];

        }

        public List<string> GetLocalizationTypes()
        {
            return map.Keys.ToList();
        }

        public List<string> GetLocalizationKeys()
        {
            return keys;
        }
        public void Merge(ILocalizationContext context)
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
                    Add(type, key, value);
                }
            }
        }
        public void Clear()
        {
            map.Clear();
            keys.Clear();
        }
        public void ClearLan(string lan)
        {
            if (map.ContainsKey(lan))
                map.Remove(lan);

            if (map.Count == 0)
                Clear();
        }
        public void Add(string lan, string key, string value)
        {
            SerializableDictionary<string, string> dic = Add(lan);
            dic[key] = value;
            if (!keys.Contains(key)) { keys.Add(key); }
        }

        public SerializableDictionary<string,string> Add(string lan)
        {
            SerializableDictionary<string, string> dic;
            if (!map.TryGetValue(lan, out dic))
            {
                dic = new SerializableDictionary<string, string>();
                map.Add(lan, dic);
            }
            return dic;
        }



        public void ClearKeys(IList<string> list)
        {
            var lanTypes = this.GetLocalizationTypes();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var key = list[i];

                for (int j = 0; j < lanTypes.Count; j++)
                {
                    var type = lanTypes[j];
                    map[type].Remove(key);
                }

                keys.Remove(key);
            }
        }


        public string FindKey(string localizationType, string val)
        {
            if (map.TryGetValue(localizationType, out var key_value))
            {
                foreach (var item in key_value)
                {
                    if (item.Value == val)
                    {
                        return item.Key;
                    }
                }
            }
            return string.Empty;
        }


    }
}

