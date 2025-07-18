﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WooLocalization
{
    public class TextValueActor_BaseEditor<T, V> : LocalizationActorEditor<T> where T : TextValueActor_Base<V>
          where V : LocalizationBehavior
    {
        private enum Mode
        {
            Nomal,
            NewKey,
            ReplaceValue
        }
        private Mode _mode;
        private string key;
        private string value;

        UnityEditorInternal.ReorderableList argList;
        private void SaveArgs(LocalizationBehavior component, T context)
        {
            context.formatArgs = argList.list as string[];
            SetDirty(component);
        }
        private void CreateList(LocalizationBehavior component, T context)
        {
            if (argList == null)
                argList = new UnityEditorInternal.ReorderableList(null, typeof(string));
            argList.multiSelect = true;
            argList.onAddCallback = (value) =>
            {
                var array = argList.list as string[];
                ArrayUtility.Add(ref array, "newArg");
                argList.list = array;
                SaveArgs(component, context);
            };
            argList.onRemoveCallback = (value) =>
            {
                var indexes = argList.selectedIndices;
                var array = argList.list as string[];

                for (int i = indexes.Count - 1; i >= 0; i--)
                {
                    ArrayUtility.RemoveAt(ref array, indexes[i]);
                }
                argList.list = array;
                argList.ClearSelection();
                SaveArgs(component, context);
                GUIUtility.ExitGUI();
            };
            argList.onChangedCallback = (value) => { SaveArgs(component, context); };
            argList.onReorderCallback = (value) => { SaveArgs(component, context); };
            argList.drawHeaderCallback = (rect) =>
            {
                GUI.Label(rect, "FormatArgs", EditorStyles.boldLabel);
            };
            argList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var src = context.formatArgs[index];
                var tmp = EditorGUI.TextField(rect, src);
                if (tmp != src)
                {
                    context.formatArgs[index] = tmp;
                    SaveArgs(component, context);
                }
            };
            argList.list = context.formatArgs;
        }

        protected override void OnGUI(LocalizationBehavior component, T context)
        {
            var language = Localization.GetLocalizationType();
            var __mode = (Mode)EditorGUILayout.EnumPopup(nameof(Mode), _mode);
            if (__mode != _mode)
            {
                _mode = __mode;
                switch (_mode)
                {
                    case Mode.Nomal:
                    case Mode.NewKey:
                        key = value = string.Empty;
                        break;
                    case Mode.ReplaceValue:
                        key = context.key;
                        //value = component.GetLocalization(key);
                        value = context.GetTargetText(component, out System.Exception _);
                        break;
                }
            }
            switch (_mode)
            {
                case Mode.Nomal:
                    {
                        var keys = component.GetLocalizationKeys();

                        if (keys == null || keys.Count == 0) return;
                        var _index = keys.IndexOf(context.key);
                        var rect = EditorGUILayout.GetControlRect();
                        GUILayout.BeginHorizontal();
                        rect = EditorGUI.PrefixLabel(rect, new GUIContent("Key"));
                        var index = LocalizationEditorHelper.AdvancedPopup(rect, _index, keys.ToArray(), 350, EditorStyles.miniPullDown);
                        GUILayout.EndHorizontal();
                        if (index >= keys.Count || index == -1)
                            index = 0;
                        if (index != _index)
                        {
                            context.SetKey(keys[index]);
                            SetDirty(component);
                        }
                    }
                    break;
                case Mode.NewKey:
                    {
                        key = EditorGUILayout.TextField(nameof(key), key);
                        value = EditorGUILayout.TextField(nameof(value), value);

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("+", GUILayout.Width(20)))
                        {
                            var data = component.context;
                            if (string.IsNullOrEmpty(key))
                            {
                                EditorWindow.focusedWindow.ShowNotification(new GUIContent("value can not be null"));
                                GUIUtility.ExitGUI();
                                return;
                            }
                            var keys = component.GetLocalizationKeys();
                            if (keys.Contains(key))
                            {
                                bool bo = EditorUtility.DisplayDialog("same key exist", $"replace \n key {key} \n" +
                                      $"value: {data.GetLocalization(language, key)} => {value}", "yes", "no");
                                if (!bo)
                                {
                                    GUIUtility.ExitGUI();
                                    return;
                                }
                            }
                            LocalizationEditorHelper.AddLocalizationPair(data, language, key, value);
                            context.SetKey(key);
                            SetDirty(component);

                        }
                        GUILayout.EndHorizontal();
                    }
                    break;
                case Mode.ReplaceValue:
                    {
                        value = EditorGUILayout.TextField(nameof(value), value);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("go", GUILayout.Width(30)))
                        {
                            var data = component.context;
                            //key = context.key;
                            bool bo = EditorUtility.DisplayDialog("same key exist", $"replace \n key {key} \n" +
                                      $"value: {data.GetLocalization(language, key)} => {value}", "yes", "no");
                            if (!bo)
                            {
                                GUIUtility.ExitGUI();
                                return;
                            }
                            LocalizationEditorHelper.AddLocalizationPair(data, language, key, value);
                            context.SetKey(key);
                            SetDirty(component);
                        }
                        GUILayout.EndHorizontal();
                    }
                    break;
            }

            CreateList(component, context);
            argList.DoLayoutList();



            System.Exception err = null;
            var result = context.GetTargetText(component, out err);
            if (err != null)
                EditorGUILayout.HelpBox(err.Message, MessageType.Error, true);
            else
                EditorGUILayout.HelpBox($"Result\n{result}", MessageType.Info, true);
      
        }





    }
}
