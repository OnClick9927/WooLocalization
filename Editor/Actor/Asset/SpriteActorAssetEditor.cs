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
    [CustomEditor(typeof(SpriteActorAsset))]
    class SpriteActorAssetEditor : ActorAssetEditor<Sprite, SpriteActorAsset>
    {

        protected override float GetRowHeight()
        {
            return 60;
        }
        protected override Sprite DrawField(Rect pos, Sprite src)
        {

            float min = Mathf.Min(pos.width, pos.height);
            pos.size = Vector2.one * min;
            return EditorGUI.ObjectField(pos, src, typeof(Sprite), false) as Sprite;
        }
    }
}
