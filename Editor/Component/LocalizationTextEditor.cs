/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;
using static WooLocalization.LocalizationText;

namespace WooLocalization
{


    [CustomEditor(typeof(LocalizationText))]
    class LocalizationTextEditor : LocalizationBehaviorEditor<LocalizationText>
    {
        [LocalizationActorEditorAttribute]

        class TextFontActorEditor : LocalizationMapActorEditor<TextFontActor, Font, LocalizationText>
        {
            protected override Type GetAssetType() => typeof(FontActorAsset);
        }
        [LocalizationActorEditorAttribute]
        class TextFontSizeActorEditor : LocalizationMapActorEditor<TextFontSizeActor, int, LocalizationText>
        {
            protected override Type GetAssetType() => typeof(IntActorAsset);

        }
        [LocalizationActorEditorAttribute]
        class TextValueActorEditor : TextValueActor_BaseEditor<TextValueActor, LocalizationText>
        {

        }
    }
}
