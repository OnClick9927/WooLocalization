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


    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Text))]
    [DisallowMultipleComponent]

    public class LocalizationText : LocalizationGraphic<UnityEngine.UI.Text>
    {
        [System.Serializable]
        public class TextFontActor : LocalizationMapActor<LocalizationText, Font>
        {
            public TextFontActor(bool enable) : base(enable)
            {
            }



            protected override void Execute(string language, LocalizationText component)
            {
                component.graphicT.font = GetValue(language);
            }
        }
        [System.Serializable]
        public class TextValueActor : TextValueActor_Base<LocalizationText>
        {
            public TextValueActor(bool enable) : base(enable) { }
            protected override string GetComponentText() => this.behavior.graphicT.text;
            protected override void SetComponentText(string value) => this.behavior.graphicT.text = value;
        }
        [System.Serializable]
        public class TextFontSizeActor : LocalizationMapActor<LocalizationText, int>
        {
            public TextFontSizeActor(bool enable) : base(enable)
            {
            }


            protected override void Execute(string language, LocalizationText component)
            {
                component.graphicT.fontSize = GetValue(language);

            }
        }
        public TextValueActor text = new TextValueActor(true);
        public TextFontActor font = new TextFontActor(false);
        public TextFontSizeActor fontSize = new TextFontSizeActor(false);
        protected override List<ILocalizationActor> GetActors()
        {
            var _base = base.GetActors();
            _base.Add(text);
            _base.Add(font);
            _base.Add(fontSize);
            return _base;
        }
    }
}
