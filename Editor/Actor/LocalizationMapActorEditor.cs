/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using UnityEditor;
using UnityEngine;

namespace WooLocalization
{
    public abstract class LocalizationMapActorEditor<Actor, Value, Behavior> : LocalizationActorEditor<Actor>
        where Actor : LocalizationMapActor<Behavior, Value>
        where Behavior : LocalizationBehavior
    {
        protected override void OnGUI(LocalizationBehavior component, Actor context)
        {
            if (component.context == null) return;
            GUI.enabled = false;
            EditorGUILayout.EnumPopup(nameof(context.mode), context.mode);
            if (context.mode != LocalizationMapActor<Behavior, Value>.Mode.Default)
            {
                EditorGUILayout.TextField(nameof(context.key), context.key);
                EditorGUILayout.TextField(nameof(context.CustomContextType), context.CustomContextType);
            }

            GUI.enabled = true;
            var keys = component.GetLocalizationTypes();
            var map = context.map;
            for (int i = 0; keys.Count > i; i++)
            {
                var key = keys[i];
                if (context.AddLocalizationTypeToMap(key))
                {
                    SetDirty(component);
                }
            }
            if (context.mode != LocalizationMapActor<Behavior, Value>.Mode.Default)
                return;
            for (int i = 0; i < keys.Count; i++)
            {
                var lan = keys[i];
                var src = map[lan];

                var tmp = Draw(lan, src);
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
                    map[lan] = tmp;
                    SetDirty(component);
                }

            }
        }
        protected virtual Value Draw(string lan, Value value)
        {
            return LocalizationEditorHelper.DrawObject(lan, value);
        }
    }
}
