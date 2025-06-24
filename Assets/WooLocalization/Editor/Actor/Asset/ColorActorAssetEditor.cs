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
    [CustomEditor(typeof(ColorActorAsset))]
    class ColorActorAssetEditor : ActorAssetEditor<Color, ColorActorAsset>
    {

        protected override Color DrawField(Rect pos, Color src)
        {
            return EditorGUI.ColorField(pos, src);
        }
    }
}
