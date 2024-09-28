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
        public SerializableDictionary<string, T> objects = new SerializableDictionary<string, T>();

        public ObjectActor(bool enable) : base(enable)
        {
        }

        public override T GetDefault()
        {
            return default;
        }

        protected override void Execute(string localizationType, LocalizationBehavior component)
        {

        }
    }
}
