/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEngine;

namespace WooLocalization
{
    [CreateAssetMenu(menuName = "WooLocalization/LocalizationData", order = -1000)]
    public class LocalizationData : ActorAsset<string>, ILocalizationContext
    {

        public void Merge(ILocalizationContext context)
        {
            var types = context.GetLocalizationTypes();
            var keys = context.GetLocalizationKeys();
            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];
                for (var j = 0; j < keys.Count; j++)
                {
                    var key = keys[j];
                    var value = context.GetLocalization(type, key);
                    AddPair(type, key, value);
                }
            }
        }
    }
}

