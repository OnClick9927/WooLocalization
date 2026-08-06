/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;

namespace WooLocalization
{
    class MixedRecorder : ILocalizationPrefRecorder
    {
        private readonly List<ILocalizationPrefRecorder> recorders = new List<ILocalizationPrefRecorder>();
        public void Add(ILocalizationPrefRecorder recorder)
        {
            if (recorder == null) return;
            if (recorders.Contains(recorder)) return;
            recorders.Add(recorder);
        }
        public LocalizationPref Read()
        {
            if (recorders.Count == 0) return new LocalizationPref();
            return recorders[0].Read();
        }

        public void Write(LocalizationPref pref)
        {
            for (int i = 0; i < recorders.Count; i++)
            {
                recorders[i].Write(pref);
            }
        }
    }
}
