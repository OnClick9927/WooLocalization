/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Collections.Generic;
namespace WooLocalization
{
    public interface ILocalizationEventActor
    {
        void OnLocalizationChange();
    }
    public static class Localization
    {
        public static string localizationType => pref.localizationType;
        public static LocalizationData context;
        private static LocalizationPref pref;
        private static ILocalizationPrefRecorder recorder = new MixedRecorder();
        private static List<ILocalizationEventActor> handlers = new List<ILocalizationEventActor>();

        public static void AddHandler(ILocalizationEventActor handler) => handlers.Add(handler);
        public static void RemoveHandler(ILocalizationEventActor handler) => handlers.Remove(handler);
        public static void SetRecorder(ILocalizationPrefRecorder recorder)
        {
            (Localization.recorder as MixedRecorder).Add(recorder);
            pref = Localization.recorder.Read();
        }

        public static void SetDefaultLocalizationType(string type)
        {
            if (string.IsNullOrEmpty(localizationType))
                SetLocalizationType(type);
        }
        public static void SetContext(LocalizationData context) => Localization.context = context;
        public static void MergeContext(LocalizationData context)
        {
            if (Localization.context == null)
                SetContext(context);
            else
                Localization.context.Merge(context);
        }
        public static void SetLocalizationType(string type)
        {
            if (localizationType == type) return;
            pref.localizationType = type;
            recorder.Write(pref);
            for (int i = 0; i < handlers.Count; i++)
            {
                var handler = handlers[i];
                if (handler == null) continue;
                handler.OnLocalizationChange();
            }
        }
        public static string GetLocalizationType() => pref.localizationType;


        public static List<string> GetLocalizationTypes(LocalizationData context)
        {
            if (context == null)
                return null;
            return context.GetLocalizationTypes();
        }
        public static List<string> GetLocalizationKeys(LocalizationData context)
        {
            if (context == null) return null;
            return context.GetLocalizationKeys();
        }

        public static List<string> GetLocalizationTypes() => GetLocalizationTypes(context);
        public static List<string> GetLocalizationKeys() => GetLocalizationKeys(context);



        public static string GetLocalization(LocalizationData context, string type, string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (context == null) return string.Empty;
            var restult = context.GetLocalization(type, key);
            if (string.IsNullOrEmpty(restult))
                return key;
            return restult;
        }
        public static string GetLocalization(LocalizationData context, string key) => GetLocalization(context, localizationType, key);
        public static string GetLocalization(string type, string key) => GetLocalization(context, type, key);
        public static string GetLocalization(string key) => GetLocalization(context, key);
    }
}
