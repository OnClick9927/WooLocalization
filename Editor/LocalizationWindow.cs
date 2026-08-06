/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-06-29
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WooLocalization
{

    class LocalizationWindow : EditorWindow
    {
        [MenuItem("Tools/WooLocalization/Window")]
        static void Open()
        {
            GetWindow<LocalizationWindow>();
        }
        private string[] translatorTypeNames = Array.Empty<string>();
        private string[] translatorDisplayNames = Array.Empty<string>();
        private IReadOnlyList<string> cachedLanguageTypes;
        private string[] languageOptions = Array.Empty<string>();

        private void OnEnable()
        {
            var types = LocalizationEditorHelper.GetTranslatorTypes();
            translatorTypeNames = new string[types.Count];
            translatorDisplayNames = new string[types.Count];
            for (var i = 0; i < types.Count; i++)
            {
                translatorTypeNames[i] = types[i].FullName;
                translatorDisplayNames[i] = types[i].Name;
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("CSV Regex");
            LocalizationSetting.quotesReg = EditorGUILayout.TextField(nameof(LocalizationSetting.quotesReg), LocalizationSetting.quotesReg);
            LocalizationSetting.lineReg = EditorGUILayout.TextField(nameof(LocalizationSetting.lineReg), LocalizationSetting.lineReg);
            LocalizationSetting.fieldReg = EditorGUILayout.TextField(nameof(LocalizationSetting.fieldReg), LocalizationSetting.fieldReg);
            EditorGUILayout.EndVertical();



            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var _type = LocalizationSetting.translatorType;
            var _index = Array.IndexOf(translatorTypeNames, _type);
            _index = Mathf.Max(0, _index);
            _index = EditorGUILayout.Popup("Translator", _index, translatorDisplayNames);
            LocalizationSetting.translatorType = translatorTypeNames[_index];
            var tranlator = LocalizationEditorHelper.GetTranslator();
            LocalizationSetting.translatorParam = tranlator.OnGUI(LocalizationSetting.translatorParam);


            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            LocalizationSetting.defaultData = EditorGUILayout.ObjectField(nameof(LocalizationSetting.defaultData), LocalizationSetting.defaultData, typeof(LocalizationData), false) as LocalizationData;

            if (LocalizationSetting.defaultData)
            {
                var types = LocalizationSetting.defaultData.GetLocalizationTypesReadOnly();
                if (types.Count != 0)
                {
                    UpdateLanguageOptions(types);
                    var index = EditorGUILayout.Popup("LanguageType", IndexOf(types, LocalizationSetting.language), languageOptions);
                    Localization.SetLocalizationType(types[Mathf.Clamp(index, 0, types.Count - 1)]);
                    //GUILayout.Label("Type Reflect", EditorStyles.boldLabel);
                    //for (var i = 0; i < types.Count; i++)
                    //{
                    //    var type = types[i];
                    //    var src = LocalizationSetting.GetLocalizationTypeReflect(type);
                    //    var tmp = EditorGUILayout.TextField(type, src);
                    //    if (tmp != src)
                    //    {
                    //        LocalizationSetting.SetLocalizationTypeReflect(type, tmp);
                    //    }
                    //}
                }


            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
            if (GUILayout.Button("Import TMP Extend"))
            {
                string path = $"{LocalizationEditorHelper.pkgPath}/Package Resources/LocalizationTmp Extend.unitypackage";
                AssetDatabase.ImportPackage(path, true);
            }
        }

        private void UpdateLanguageOptions(IReadOnlyList<string> types)
        {
            var needsRefresh = !ReferenceEquals(cachedLanguageTypes, types) || languageOptions.Length != types.Count;
            if (!needsRefresh)
            {
                for (var i = 0; i < types.Count; i++)
                {
                    if (!string.Equals(languageOptions[i], types[i], StringComparison.Ordinal))
                    {
                        needsRefresh = true;
                        break;
                    }
                }
            }

            if (!needsRefresh)
                return;
            cachedLanguageTypes = types;
            languageOptions = new string[types.Count];
            for (var i = 0; i < types.Count; i++)
                languageOptions[i] = types[i];
        }

        private static int IndexOf(IReadOnlyList<string> values, string value)
        {
            for (var i = 0; i < values.Count; i++)
            {
                if (string.Equals(values[i], value, StringComparison.Ordinal))
                    return i;
            }
            return -1;
        }




    }
}
