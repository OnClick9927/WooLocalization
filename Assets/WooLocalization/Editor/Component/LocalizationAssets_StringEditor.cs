﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEditor;

namespace WooLocalization
{
    [CustomEditor(typeof(LocalizationAssets_String))]
    class LocalizationAssets_StringEditor : LocalizationAssetsEditor<LocalizationAssets_String, string>
    {

    }
}
