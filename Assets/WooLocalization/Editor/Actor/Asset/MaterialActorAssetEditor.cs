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
    [CustomEditor(typeof(MaterialActorAsset))]
    class MaterialActorAssetEditor : ActorAssetEditor<Material, MaterialActorAsset>
    {

        protected override Material DrawField(Rect pos, Material src)
        {
            return EditorGUI.ObjectField(pos, src, typeof(Material), false) as Material;
        }
    }
}
