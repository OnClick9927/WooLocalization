/*********************************************************************************
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
    class LocalizationAssetsEditor<V, T> : LocalizationBehaviorEditor<LocalizationAssets<T>>
        where V : LocalizationAssets<T>
        
    {
        private string _name = "";

        protected override void RemoveActor(ILocalizationActor actor)
        {
            comp.objects.Remove(actor as ObjectActor<T>);
        }
        protected override void DrawInspectorGUI()
        {
            DrawContext();
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label($"New ObjectActor<{typeof(T)}>");
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            _name = EditorGUILayout.TextField("Name", _name);
            if (GUILayout.Button("+", GUILayout.Width(50)))
            {
                if (string.IsNullOrEmpty(_name))
                {
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("Name is Null"));
                    return;
                }
                if (comp.objects.Any(x => (x as ILocalizationActor).name == _name))
                {
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("Same Name"));
                    return;
                }
                comp.objects.Add(new ObjectActor<T>(true).SetName(_name).SetCanRemove(true) as ObjectActor<T>);
                EditorUtility.SetDirty(comp);
                AssetDatabase.SaveAssetIfDirty(comp);
                LoadFields();

            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.EndVertical();
            DrawFields();
        }
    }
}
