/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using static WooLocalization.LocalizationPrefab;

namespace WooLocalization
{

    [CustomEditor(typeof(LocalizationPrefab))]
    class LocalizationPrefabEditor : LocalizationBehaviorEditor<LocalizationPrefab>
    {
        [LocalizationActorEditorAttribute]

        class PrefabActorEditor : LocalizationMapActorEditor<PrefabActor, GameObject, LocalizationBehavior>
        {
            protected override bool NeedExecute() => false;
            protected override GameObject Draw(string lan, GameObject value) => EditorGUILayout.ObjectField(lan, value, typeof(GameObject), false) as GameObject;
        }
    }
}
