/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace WooLocalization
{
    [UnityEngine.RequireComponent(typeof(TMPro.TMP_Text))]
    [DisallowMultipleComponent]
    public class LocalizationTMP_Text : LocalizationGraphic<TMPro.TMP_Text>
    {
        [System.Serializable]
        public class TMPTextActor : LocalizationActor<LocalizationTMP_Text>
        {

            public string key;
            private string _lastKey;
            public string[] formatArgs = new string[0];

            public TMPTextActor(bool enable) : base(enable)
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
            protected override void Execute(string localizationType, LocalizationTMP_Text component)
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
        public class TMPFontActor : LocalizationMapActor<LocalizationTMP_Text, TMP_FontAsset>
        {
            public TMPFontActor(bool enable) : base(enable)
            {
                
            }

            public override TMP_FontAsset GetDefault()
            {
                if (TMP_Settings.instance != null)
                    return TMP_Settings.defaultFontAsset;
                return null;
            }

            protected override void Execute(string localizationType, LocalizationTMP_Text component)
            {
                component.graphicT.font = GetValue(localizationType);
            }
        }

        [System.Serializable]
        public class TMPFontSizeActor : LocalizationMapActor<LocalizationTMP_Text, float>
        {
            public TMPFontSizeActor(bool enable) : base(enable)
            {
            }

            public override float GetDefault()
            {
                return 36;
            }

            protected override void Execute(string localizationType, LocalizationTMP_Text component)
            {
                component.graphicT.fontSize = GetValue(localizationType);
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
