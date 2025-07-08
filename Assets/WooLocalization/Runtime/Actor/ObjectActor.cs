/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

namespace WooLocalization
{
    [System.Serializable]
    public class ObjectActor<T> : LocalizationMapActor<LocalizationBehavior, T>
    {
        public ObjectActor(bool enable) : base(enable)
        {
        }
        protected override void Execute(string language, LocalizationBehavior component)
        {

        }

        public override T GetDefault() => default;
    }
}
