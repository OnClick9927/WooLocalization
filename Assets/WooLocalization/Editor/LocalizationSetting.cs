/*********************************************************************************
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


        private static LocalizationSetting context => LocalizationEditorHelper.context;


        LocalizationPref ILocalizationPrefRecorder.Read()
        {
            return new LocalizationPref()
            {
                language = language,
            };
        }

        void ILocalizationPrefRecorder.Write(LocalizationPref pref)
        {
            this._localizationType = pref.language;
            Save();
        }
        [UnityEngine.SerializeField] private string _localizationType = "CN";
        [UnityEngine.SerializeField] private string _defaultData;
        [UnityEngine.SerializeField] private string _lineReg = "\"";
        [UnityEngine.SerializeField] private string _fieldReg = "\\G(?:^|,)(?:\"((?>[^\"]*)(?>\"\"[^\"]*)*)\"|([^\",]*))";
        [UnityEngine.SerializeField] private string _quotesReg = "\"\"";
        [UnityEngine.SerializeField] private string _translatorType;
        [UnityEngine.SerializeField] private SerializableDictionary<string, string> _translatorParam = new SerializableDictionary<string, string>();

        private void Save()
        {
            LocalizationEditorHelper.SaveContext(this);
        }
        [UnityEngine.SerializeField] private string _lastCSVPath = "Assets";
        public static string lastCSVPath
        {
            get { return context._lastCSVPath; }
            set
            {
                if (context._lastCSVPath != value)
                {
                    context._lastCSVPath = value;
                    context.Save();
                }

            }
        }

        public static string translatorType
        {
            get { return context._translatorType; }
            set
            {
                if (context._translatorType == value) return;
                context._translatorType = value;
                context.Save();

            }
        }
        public static string translatorParam
        {
            get
            {
                if (string.IsNullOrEmpty(translatorType))
                    return string.Empty;
                string result = string.Empty;
                context._translatorParam.TryGetValue(translatorType, out result);
                return result;
            }
            set
            {
                string result = string.Empty;
                context._translatorParam.TryGetValue(translatorType, out result);
                if (result != value)
                {
                    context._translatorParam[translatorType] = value;
                    context.Save();
                }

            }
        }


        public static string language
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

        private static LocalizationData __defaultData;
        public static LocalizationData defaultData
        {
            get
            {
                if (__defaultData == null)
                {
                    __defaultData = AssetDatabase.LoadAssetAtPath<LocalizationData>(context._defaultData);
                }
                LocalizationBehavior.defaultContext = __defaultData;
                return __defaultData;
            }
            set
            {
                var path = AssetDatabase.GetAssetPath(value);
                if (context._defaultData == path) return;
                __defaultData = null;
                context._defaultData = path;

                context.Save();
                LocalizationBehavior.defaultContext = __defaultData;

            }
        }


        //public static string GetLocalizationTypeReflect(string src)
        //{
        //    if (context.reflect.ContainsKey(src))
        //        return context.reflect[src];
        //    return src;
        //}
        //public static void SetLocalizationTypeReflect(string src, string dst)
        //{
        //    if (context.reflect.ContainsKey(src))
        //        context.reflect[src] = dst;
        //    else
        //        context.reflect.Add(src, dst);
        //    context.Save();
        //}
    }
}
