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
    [CustomEditor(typeof(GameObjectActorAsset))]
    class GameObjectActorAssetEditor : ActorAssetEditor<GameObject, GameObjectActorAsset>
    {
        protected override GameObject DrawField(Rect pos, GameObject src)
        {
            return EditorGUI.ObjectField(pos, src, typeof(GameObject), false) as GameObject;
        }
    }
}
