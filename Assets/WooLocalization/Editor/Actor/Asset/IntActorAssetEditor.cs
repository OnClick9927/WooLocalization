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
    [CustomEditor(typeof(IntActorAsset))]
    class IntActorAssetEditor : ActorAssetEditor<int, IntActorAsset>
    {

        protected override int DrawField(Rect pos, int src)
        {
            return EditorGUI.IntField(pos, src);
        }
    }
}
