﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;
using static WooLocalization.LocalizationImage;

namespace WooLocalization
{
    [CustomEditor(typeof(LocalizationImage))]
    class LocalizationImageEditor : LocalizationBehaviorEditor<LocalizationImage>
    {
        [LocalizationActorEditorAttribute]
        class ImageSpriteActorEditor : LocalizationMapActorEditor<ImageSpriteActor, Sprite, LocalizationImage>
        {

            protected override Type GetAssetType() => typeof(SpriteActorAsset);

        }
    }
}
