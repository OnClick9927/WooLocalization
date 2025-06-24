/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
#if UNITY_EDITOR
using System;
using UnityEditor;
namespace WooLocalization
{

    [CustomEditor(typeof(LocalizationTMP_Text))]
    class LocalizationTMP_TextEditor : LocalizationBehaviorEditor<LocalizationTMP_Text>
    {

        [LocalizationActorEditorAttribute]
        class TMPTextActorEditor : TextValueActor_BaseEditor<LocalizationTMP_Text.TMPTextActor, LocalizationTMP_Text>
        {
         
        }

        [LocalizationActorEditorAttribute]
        class TextFontActorEditor : LocalizationMapActorEditor<LocalizationTMP_Text.TMPFontActor, TMPro.TMP_FontAsset, LocalizationTMP_Text>
        {
            protected override Type GetAssetType() => typeof(TMPFontAsset);

        }

        [LocalizationActorEditorAttribute]
        class TMPFontSizeActorEditor : LocalizationMapActorEditor<LocalizationTMP_Text.TMPFontSizeActor, float, LocalizationTMP_Text>
        {
            protected override Type GetAssetType() => typeof(FloatActorAsset);
        }
    }
}
#endif