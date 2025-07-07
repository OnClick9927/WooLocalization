/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-06-29
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
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
        //List<Type> types = new List<Type>();
        List<string> types_string = new List<string>();
        List<string> types_string_short = new List<string>();

        private void OnEnable()
        {
            var types = LocalizationEditorHelper.GetTranslatorTypes();
            types_string = types.ConvertAll(x => x.FullName);
            types_string_short = types.ConvertAll(x => x.Name);

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
            var _index = types_string.IndexOf(_type);
            _index = Mathf.Max(0, _index);
            _index = EditorGUILayout.Popup("Translator", _index, types_string_short.ToArray());
            LocalizationSetting.translatorType = types_string[_index];
            var tranlator = LocalizationEditorHelper.GetTranslator();
            LocalizationSetting.translatorParam = tranlator.OnGUI(LocalizationSetting.translatorParam);


            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            LocalizationSetting.defaultData = EditorGUILayout.ObjectField(nameof(LocalizationSetting.defaultData), LocalizationSetting.defaultData, typeof(LocalizationData), false) as LocalizationData;

            if (LocalizationSetting.defaultData)
            {
                var types = LocalizationSetting.defaultData.GetLocalizationTypes();
                if (types.Count != 0)
                {
                    var index = EditorGUILayout.Popup("LanguageType", types.IndexOf(LocalizationSetting.language), types.ToArray());
                    Localization.SetLocalizationType(types[Mathf.Clamp(index, 0, types.Count)]);
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




    }
}
