/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEngine.UI;

namespace WooLocalization
{
    public static class LocalizationEx
    {
        public static void SetLocalization(this Text tmp, string key, params string[] args)
        {
            var comp = tmp.GetComponent<WooLocalization.LocalizationText>();
            if (comp == null)
                comp = tmp.gameObject.AddComponent<WooLocalization.LocalizationText>();
            comp.LoadActors();
            comp.text.formatArgs = args;
            comp.text.SetKey(key);
        }
    }
}
