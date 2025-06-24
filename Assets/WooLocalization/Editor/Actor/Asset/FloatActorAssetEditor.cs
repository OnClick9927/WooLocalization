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
    [CustomEditor(typeof(FloatActorAsset))]
    class FloatActorAssetEditor : ActorAssetEditor<float, FloatActorAsset>
    {
        protected override float DrawField(Rect pos, float src)
        {
            return EditorGUI.FloatField(pos, src);
        }

    }
}
