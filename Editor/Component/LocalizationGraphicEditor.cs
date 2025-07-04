/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;
using static WooLocalization.LocalizationGraphic;

namespace WooLocalization
{
    [CustomEditor(typeof(LocalizationGraphic))]
    class LocalizationGraphicEditor : LocalizationBehaviorEditor<LocalizationGraphic>
    {
        [LocalizationActorEditorAttribute]
        class GraphicColorActorEditor : LocalizationMapActorEditor<GraphicColorActor, Color, LocalizationGraphic>
        {
            protected override Type GetAssetType() => typeof(ColorActorAsset);
            protected override Color GetDefault() => Color.white;
        }
        [LocalizationActorEditorAttribute]
        class GraphicMaterialActorEditor : LocalizationMapActorEditor<GraphicMaterialActor, Material, LocalizationGraphic>
        {
            protected override Type GetAssetType() => typeof(MaterialActorAsset);
            protected override Material GetDefault() => UnityEngine.UI.Graphic.defaultGraphicMaterial;
        }
    }
}
