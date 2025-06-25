/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Text.RegularExpressions;

namespace WooLocalization
{
    public abstract class TextValueActor_Base<T> : LocalizationActor<T> where T : LocalizationBehavior
    {
        [UnityEngine.SerializeField] private string _key;
        public string key => _key;
        public string[] formatArgs = new string[0];

        public TextValueActor_Base(bool enable) : base(enable)
        {

        }
        protected abstract string GetComponentText();
        protected abstract void SetComponentText(string value);

        protected sealed override void OnEditorLoad()
        {
            if (string.IsNullOrEmpty(key))
            {

                var txt = GetComponentText();
                if (!string.IsNullOrEmpty(txt))
                {
                    var key = this.behavior.FindKey(Localization.localizationType, txt);
                    if (!string.IsNullOrEmpty(key))
                        SetKey(key);
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
            catch (Exception ex)
            {
                err = ex;
                return format;
            }
        }
        protected sealed override void Execute(string localizationType, T component)
        {
            Exception err;
            SetComponentText(GetTargetText(component, out err));
            if (err != null)
                throw err;
        }
        public void SetKey(string key)
        {
            this._key = key;
            SetDirty();
            ((ILocalizationActor)this).enable = true;
            ((ILocalizationActor)this).Execute();
        }

    }
}
