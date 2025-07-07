/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;

namespace WooLocalization
{
    [System.Serializable]
    public class TranslateResultCollection
    {
        public int errorCode;

        public List<TranslateResult> translateResults;
    }
}
