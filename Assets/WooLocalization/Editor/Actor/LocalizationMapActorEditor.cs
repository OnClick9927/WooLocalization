﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using System;
using UnityEditor;
using UnityEngine;

namespace WooLocalization
{
    public abstract class LocalizationMapActorEditor<Actor, Value, Behavior> : LocalizationActorEditor<Actor>
        where Actor : LocalizationMapActor<Behavior, Value>
        where Behavior : LocalizationBehavior
    {

        protected abstract Type GetAssetType();
        protected abstract Value GetDefault();
        public void EnsureMap(Behavior component, Actor context)
        {
            var laguages = component.GetLocalizationTypes();
            for (int i = 0; laguages.Count > i; i++)
            {
                var language = laguages[i];
                //context.AddLocalizationTypeToMap(language, GetDefault());
                context.AddLocalizationTypeToMap(language, GetDefault());
            }
            SetDirty(component);
        }
        protected override void OnGUI(LocalizationBehavior component, Actor context)
        {
            if (component.context == null) return;

            var last_mode = context.mode;
            var _mode = (LocalizationMapActor<Behavior, Value>.Mode)EditorGUILayout.EnumPopup(nameof(context.mode), context.mode);
            var last_key = context.key;
            if (context.mode != LocalizationMapActor<Behavior, Value>.Mode.Default)
            {
                if (context.mode == LocalizationMapActor<Behavior, Value>.Mode.Custom)
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField(nameof(context.CustomContextType), context.CustomContextType);
                    GUI.enabled = true;
                }

                if (context.mode == LocalizationMapActor<Behavior, Value>.Mode.Asset)
                {

                    context.asset = EditorGUILayout.ObjectField(nameof(context.asset), context.asset, GetAssetType(), false) as ActorAsset<Value>;

                    if (context.asset != null)
                    {

                        var keys = context.asset.GetLocalizationKeys();

                        if (keys == null || keys.Count == 0) return;
                        var _index = keys.IndexOf(last_key);
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
                        using (new EditorGUI.DisabledScope(true))
                            LocalizationEditorHelper.DrawObject("Preview", context.GetValue());
                    }


                }
                else
                {
                    last_key = EditorGUILayout.TextField(nameof(context.key), last_key);

                }
            }


            if (!EditorApplication.isPlaying)
            {
                if (_mode != LocalizationMapActor<Behavior, Value>.Mode.Custom)
                {
                    context.SetMode(_mode);
                }
            }






            //for (int i = 0; laguages.Count > i; i++)
            //{
            //    var language = laguages[i];
            //    //context.AddLocalizationTypeToMap(language, GetDefault());
            //    if (context.AddLocalizationTypeToMap(language, GetDefault()))
            //    {
            //        SetDirty(component);
            //    }
            //}
            if (context.mode != LocalizationMapActor<Behavior, Value>.Mode.Default)
                return;
            var laguages = component.GetLocalizationTypes();
            var map = context.map;
            for (int i = 0; i < laguages.Count; i++)
            {
                var language = laguages[i];
                var src = map[language];

                var tmp = LocalizationEditorHelper.DrawObject(language, src);
                bool change = false;
                if (src == null)
                {
                    if (tmp != null) change = true;
                }
                else
                {
                    if (tmp == null) change = true;
                    else change = !src.Equals(tmp);
                }
                if (change)
                {
                    map[language] = tmp;
                    SetDirty(component);
                }

            }
        }

    }
}
