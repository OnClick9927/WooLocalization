/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

            public override Font GetDefault()
            {
                return Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            protected override void Execute(string localizationType, LocalizationText component)
            {
                component.graphicT.font = GetValue(localizationType);
            }
        }
        [System.Serializable]
        public class TextValueActor : LocalizationActor<LocalizationText>
        {
            public string key;
            private string _lastKey;
            public string[] formatArgs = new string[0];

            public TextValueActor(bool enable) : base(enable)
            {

            }
            protected override void OnAddComponent()
            {
                if (string.IsNullOrEmpty(key))
                {

                    var txt = this.behavior.graphicT.text;
                    if (!string.IsNullOrEmpty(txt))
                    {
                        var contxt = this.behavior.context;
                        if (contxt != null)
                        {
                            var key = contxt.FindKey(Localization.localizationType, txt);
                            SetKey(key);
                        }
                    }
                }
            }
            private static Regex regex = new Regex("^{[0-9]*}$");

            public string GetTargetText(LocalizationBehavior component, out Exception err)
            {
                err = null;
                var format = component.GetLocalization(key);
                if (regex.Match(format) == null) return format;
                try
                {
                    return string.Format(format, formatArgs);
                }
                catch (System.Exception ex)
                {
                    err = ex;
                    return format;
                }
            }
            protected override void Execute(string localizationType, LocalizationText component)
            {
                _lastKey = key;
                Exception err;
                component.graphicT.text = GetTargetText(component, out err);
                if (err != null)
                    throw err;

            }
            public void SetKey(string key)
            {
                this.key = key;
                ((ILocalizationActor)this).enable = true;
                ((ILocalizationActor)this).Execute();
            }
            protected override bool NeedExecute(string localizationType)
            {
                var _base = base.NeedExecute(localizationType);
                bool self = _lastKey != this.key;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    self = true;
#endif
                return self || _base;
            }
        }
        [System.Serializable]
        public class TextFontSizeActor : LocalizationMapActor<LocalizationText, int>
        {
            public TextFontSizeActor(bool enable) : base(enable)
            {
            }

            public override int GetDefault()
            {
                return 14;
            }

            protected override void Execute(string localizationType, LocalizationText component)
            {
                component.graphicT.fontSize = GetValue(localizationType);

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
