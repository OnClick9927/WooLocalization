/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEngine;

namespace WooLocalization
{

    public abstract class LocalizationMapActor<Behavior, Value> : LocalizationActor<Behavior> where Behavior : LocalizationBehavior
    {
        [System.Serializable]
        public class MapContext : IMapActorContext<Value>
        {
            internal SerializableDictionary<string, Value> map => actor.map;
            internal LocalizationMapActor<Behavior, Value> actor;
            public Value GetValue(string localizationType, string key)
            {
                Value v;
                map.TryGetValue(localizationType, out v);
                return v;
            }

            internal bool AddLocalizationTypeToMap(string localizationType, Value _default)
            {
                Value value;
                if (!map.TryGetValue(localizationType, out value))
                {
                    map.Add(localizationType, _default);
                    return true;
                }
                return false;
            }
        }
        public enum Mode
        {
            Default, Custom
        }
        public SerializableDictionary<string, Value> map = new SerializableDictionary<string, Value>();
        public Mode mode { get; private set; }
        public string key { get; private set; }


        [SerializeField] private MapContext context = new MapContext();
        private IMapActorContext<Value> customContext;
        public string CustomContextType { get; private set; }
        public void SetCustomContext(IMapActorContext<Value> context, string key)
        {
            customContext = context;
            if (context != null)
            {
                CustomContextType = context.GetType().Name;
                mode = Mode.Custom;
                this.key = key;
            }
        }
        protected LocalizationMapActor(bool enable) : base(enable)
        {
            context.actor = this;
        }
        protected override void OnAddComponent()
        {
            context.actor = this;
            base.OnAddComponent();
        }
        public bool AddLocalizationTypeToMap(string localizationType)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return false;
            return context.AddLocalizationTypeToMap(localizationType, GetDefault());
#endif
            return false;
        }
        protected sealed override void BeforeExecute(string localizationType)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                AddLocalizationTypeToMap(localizationType);
#endif
        }
        protected virtual Value GetDefault() => default;
        public Value GetValue()
        {
            return GetValue(Localization.GetLocalizationType());
        }
        public Value GetValue(string localizationType)
        {
            if (mode == Mode.Custom)
                return customContext.GetValue(localizationType, key);
            return context.GetValue(localizationType, key);
        }
    }
}
