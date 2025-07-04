/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WooLocalization
{
    [UnityEngine.RequireComponent(typeof(TMPro.TMP_Text))]
    [DisallowMultipleComponent]
    public class LocalizationTMP_Text : LocalizationGraphic<TMPro.TMP_Text>
    {
        [System.Serializable]
        public class TMPTextActor : TextValueActor_Base<LocalizationTMP_Text>
        {
            public TMPTextActor(bool enable) : base(enable) { }
            protected override string GetComponentText() => this.behavior.graphicT.text;
            protected override void SetComponentText(string value) => this.behavior.graphicT.text = value;
        }
        [System.Serializable]
        public class TMPFontActor : LocalizationMapActor<LocalizationTMP_Text, TMP_FontAsset>
        {
            public TMPFontActor(bool enable) : base(enable)
            {
                
            }


            protected override void Execute(string language, LocalizationTMP_Text component)
            {
                component.graphicT.font = GetValue(language);
            }
        }

        [System.Serializable]
        public class TMPFontSizeActor : LocalizationMapActor<LocalizationTMP_Text, float>
        {
            public TMPFontSizeActor(bool enable) : base(enable)
            {
            }


            protected override void Execute(string language, LocalizationTMP_Text component)
            {
                component.graphicT.fontSize = GetValue(language);
            }
        }
        public TMPTextActor text = new TMPTextActor(true);
        public TMPFontActor font = new TMPFontActor(false);
        public TMPFontSizeActor fontSize = new TMPFontSizeActor(false);


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
