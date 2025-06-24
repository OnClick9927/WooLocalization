/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
namespace WooLocalization
{
    [CustomEditor(typeof(TMPFontAsset))]
    class TMPFontAssetEditor : ActorAssetEditor<TMP_FontAsset, TMPFontAsset>
    {
        protected override TMP_FontAsset DrawField(Rect pos, TMP_FontAsset src)
        {
            return EditorGUI.ObjectField(pos, src, typeof(TMP_FontAsset), false) as TMP_FontAsset;
        }
    }
}
#endif