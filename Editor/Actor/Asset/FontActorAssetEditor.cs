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
    [CustomEditor(typeof(FontActorAsset))]
    class FontActorAssetEditor : ActorAssetEditor<Font, FontActorAsset>
    {

        protected override Font DrawField(Rect pos, Font src)
        {
            return EditorGUI.ObjectField(pos, src, typeof(Font), false) as Font;
        }
    }
}
