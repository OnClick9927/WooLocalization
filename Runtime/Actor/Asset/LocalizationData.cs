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
    public class LocalizationData : ActorAsset<string>
    {

        public void Merge(LocalizationData context)
        {
            Merge((IActorContext<string>)context);
        }
    }
}

