/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
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
            protected override Type GetAssetType() => typeof(GameObjectActorAsset);

            protected override bool NeedExecute() => false;

        }
    }
}
