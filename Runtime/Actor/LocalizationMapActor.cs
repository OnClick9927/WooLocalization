/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
namespace WooLocalization
{
    public abstract class LocalizationMapActor<Behavior, Value> : LocalizationActor<Behavior> where Behavior : LocalizationBehavior
    {
        public SerializableDictionary<string, Value> map = new SerializableDictionary<string, Value>();

        protected LocalizationMapActor(bool enable) : base(enable)
        {
        }
        public bool AddLocalizationTypeToMap(string localizationType)
        {
            Value value;
            if (!map.TryGetValue(localizationType, out value))
            {
                map.Add(localizationType, GetDefault());
                return true;
            }
            return false;
        }
        protected sealed override void BeforeExecute(string localizationType)
        {
            AddLocalizationTypeToMap(localizationType);
        }
        protected virtual Value GetDefault() => default;
        public Value GetValue()
        {
            return GetValue(Localization.GetLocalizationType());
        }
        public Value GetValue(string localizationType)
        {
            Value v;
            map.TryGetValue(localizationType, out v);
            return v;
        }
    }
}
