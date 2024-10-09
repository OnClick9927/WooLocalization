﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace WooLocalization
{
    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Image))]
    [DisallowMultipleComponent]
    public class LocalizationImage : LocalizationGraphic<UnityEngine.UI.Image>
    {

        [System.Serializable]
        public class ImageSpriteActor : LocalizationMapActor<LocalizationImage, UnityEngine.Sprite>
        {
            public ImageSpriteActor(bool enable) : base(enable)
            {
            }



            protected override void Execute(string localizationType, LocalizationImage component)
            {
                component.graphicT.sprite = GetValue(localizationType);
            }
        }
        public ImageSpriteActor sprite = new ImageSpriteActor(true);

        protected override List<ILocalizationActor> GetActors()
        {
            var _base = base.GetActors();
            _base.Add(sprite);
            return _base;

        }
    }
}
