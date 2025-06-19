/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using TMPro;

namespace WooLocalization
{
    public static class LocalizationEx
    {
        public static void SetLocalization(this TextMeshProUGUI tmp, string key, params string[] args)
        {
            var comp = tmp.GetComponent<WooLocalization.LocalizationTMP_Text>();
            if (comp == null)
                comp = tmp.gameObject.AddComponent<WooLocalization.LocalizationTMP_Text>();
            comp.text.formatArgs = args;
            comp.text.SetKey(key);
        }
    }
}
