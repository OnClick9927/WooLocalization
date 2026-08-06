/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
namespace WooLocalization
{
    public static class Localization
    {
        public static string language => pref.language;
        public static LocalizationData context;
        private static LocalizationPref pref = new LocalizationPref();
        private static readonly MixedRecorder recorder = new MixedRecorder();
        private static readonly List<ILocalizationEventBehavior> behaviors = new List<ILocalizationEventBehavior>();

        public static void AddBehavior(ILocalizationEventBehavior behavior) => behaviors.Add(behavior);
        public static void RemoveBehavior(ILocalizationEventBehavior behavior) => behaviors.Remove(behavior);

        public static void ForceRefreshBehaviors()
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                var behavior = behaviors[i];
                behavior.SetActorsDirty();
                behavior.OnLanguageChange();
            }
        }


        public static void SetRecorder(ILocalizationPrefRecorder recorder)
        {
            Localization.recorder.Add(recorder);
            pref = Localization.recorder.Read() ?? new LocalizationPref();
        }

        public static void SetDefaultLocalizationType(string type)
        {
            if (string.IsNullOrEmpty(language))
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
        public static void SetLocalizationType(string language)
        {
            if (Localization.language == language) return;
            pref.language = language;
            recorder.Write(pref);
            for (int i = 0; i < behaviors.Count; i++)
            {
                var handler = behaviors[i];
                if (handler == null) continue;
                handler.OnLanguageChange();
            }
        }
        public static string GetLocalizationType() => pref.language;


        public static List<string> GetLocalizationTypes<T>(ActorAsset<T> context) => context?.GetLocalizationTypes();
        public static IReadOnlyList<string> GetLocalizationTypesReadOnly<T>(ActorAsset<T> context) => context?.GetLocalizationTypesReadOnly();
        public static List<string> GetLocalizationKeys<T>(ActorAsset<T> context) => context?.GetLocalizationKeys();

        public static List<string> GetLocalizationTypes() => GetLocalizationTypes(context);
        public static IReadOnlyList<string> GetLocalizationTypesReadOnly() => GetLocalizationTypesReadOnly(context);
        public static List<string> GetLocalizationKeys() => GetLocalizationKeys(context);


        public static T GetAssetLocalization<T>(ActorAsset<T> context, string language, string key)
        {
            if (string.IsNullOrEmpty(key)) return default;
            if (context == null) return default;
            return context.GetLocalization(language, key);

        }
        public static T GetAssetLocalization<T>(ActorAsset<T> context, string key) => GetAssetLocalization(context, language, key);

        public static string GetLocalization(LocalizationData context, string language, string key, params object[] args)
        {
            var restult = GetAssetLocalization(context, language, key);
            if (string.IsNullOrEmpty(restult)) return key;

#if UNITY_EDITOR
            var formatArgumentCount = CountFormatArguments(restult);
            if (formatArgumentCount != 0 && (args == null || args.Length != formatArgumentCount))
                throw new System.Exception($"Args Err \t\t{nameof(language)}:{language}\t{nameof(key)}:{key}\n{restult}");
#endif
            if (args == null || args.Length == 0)
                return restult;
            return string.Format(restult, args);
        }

        public static string GetLocalization(LocalizationData context, string key, params object[] args) => GetLocalization(context, language, key, args);
        public static string GetLocalization(string language, string key, params object[] args) => GetLocalization(context, language, key, args);
        public static string GetLocalization(string key, params object[] args) => GetLocalization(context, key, args);

#if UNITY_EDITOR
        private static int CountFormatArguments(string value)
        {
            var count = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != '{')
                    continue;

                var end = i + 1;
                while (end < value.Length && value[end] >= '0' && value[end] <= '9')
                    end++;
                if (end < value.Length && value[end] == '}')
                {
                    count++;
                    i = end;
                }
            }
            return count;
        }
#endif
    }
}
