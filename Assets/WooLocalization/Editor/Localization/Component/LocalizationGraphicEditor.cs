﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
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

        }
        [LocalizationActorEditorAttribute]
        class GraphicMaterialActorEditor : LocalizationMapActorEditor<GraphicMaterialActor, Material, LocalizationGraphic>
        {
  
        }
    }
}