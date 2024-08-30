﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-06-29
*********************************************************************************/

using UnityEditor;

namespace WooLocalization
{


    class LocalizationSetting : UnityEngine.ScriptableObject, ILocalizationPrefRecorder
    {


        private static LocalizationSetting context => LocalizationEditor.context;


        LocalizationPref ILocalizationPrefRecorder.Read()
        {
            return new LocalizationPref()
            {
                localizationType = localizationType,
            };
        }

        void ILocalizationPrefRecorder.Write(LocalizationPref pref)
        {
            this._localizationType = pref.localizationType;
            Save();
        }
        [UnityEngine.SerializeField] private string _localizationType = "CN";
        [UnityEngine.SerializeField] private string _defaultData;
        [UnityEngine.SerializeField] private string _lineReg = "\"";
        [UnityEngine.SerializeField] private string _fieldReg = "\\G(?:^|,)(?:\"((?>[^\"]*)(?>\"\"[^\"]*)*)\"|([^\",]*))";
        [UnityEngine.SerializeField] private string _quotesReg = "\"\"";
        [UnityEngine.SerializeField] private string _youDaoAppId = "";
        [UnityEngine.SerializeField] private string _youDaoAppSecret = "";

        [UnityEngine.SerializeField] private SerializableDictionary<string, string> reflect = new SerializableDictionary<string, string>();


        private void Save()
        {
            LocalizationEditor.SaveContext(this);
        }

        public static string localizationType
        {
            get { return context._localizationType; }
            set
            {
                if (context._localizationType == value) return;
                context._localizationType = value;
                context.Save();

            }
        }
        public static string lineReg
        {
            get { return context._lineReg; }
            set
            {
                if (context._lineReg == value) return; context._lineReg = value;
                context.Save();
            }
        }
        public static string fieldReg
        {
            get { return context._fieldReg; }
            set
            {
                if (context._fieldReg == value) return;
                context._fieldReg = value;
                context.Save();
            }
        }
        public static string quotesReg
        {
            get { return context._quotesReg; }
            set
            {
                if (context._quotesReg == value) return;
                context._quotesReg = value;
                context.Save();
            }
        }
        public static string youDaoAppId
        {
            get { return context._youDaoAppId; }
            set
            {
                if (context._youDaoAppId == value) return;
                context._youDaoAppId = value;
                context.Save();
            }
        }
        public static string youDaoAppSecret
        {
            get { return context._youDaoAppSecret; }
            set
            {
                if (context._youDaoAppSecret == value) return;
                context._youDaoAppSecret = value;
                context.Save();
            }
        }

        private static LocalizationData __defaultData;
        public static LocalizationData defaultData
        {
            get
            {
                if (__defaultData == null)
                {
                    __defaultData = AssetDatabase.LoadAssetAtPath<LocalizationData>(context._defaultData);
                }

                return __defaultData;
            }
            set
            {
                var path = AssetDatabase.GetAssetPath(value);
                if (context._defaultData == path) return;
                __defaultData = null;
                context._defaultData = path;
                context.Save();

            }
        }


        public static string GetLocalizationTypeReflect(string src)
        {
            if (context.reflect.ContainsKey(src))
                return context.reflect[src];
            return src;
        }
        public static void SetLocalizationTypeReflect(string src, string dst)
        {
            if (context.reflect.ContainsKey(src))
                context.reflect[src] = dst;
            else
                context.reflect.Add(src, dst);
            context.Save();
        }
    }
}
