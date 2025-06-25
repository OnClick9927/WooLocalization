/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace WooLocalization
{

    public abstract class LocalizationMapActor<Behavior, Value> : LocalizationActor<Behavior> where Behavior : LocalizationBehavior
    {
        [System.Serializable]
        class MapContext : IActorContext<Value>
        {
            internal SerializableDictionary<string, Value> map => actor.map;
            internal LocalizationMapActor<Behavior, Value> actor;
            Value IActorContext<Value>.GetLocalization(string localizationType, string key)
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

            List<string> IActorContext<Value>.GetLocalizationKeys() => null;

            List<string> IActorContext<Value>.GetLocalizationTypes() => actor.behavior.GetLocalizationTypes();

            void IActorContext<Value>.Merge(IActorContext<Value> context) { }
        }
        internal enum Mode
        {
            Default, Custom, Asset
        }
        [SerializeField]
        internal SerializableDictionary<string, Value> map = new SerializableDictionary<string, Value>();

        [SerializeField]
        private Mode _mode;
        internal Mode mode => _mode;

        internal void SetMode(Mode mode)
        {
            this._mode = mode;
            SetDirty();
        }

        [SerializeField]
        private string _key;
        public string key => _key;


        [SerializeField] private MapContext context = new MapContext();
        [SerializeField]
        internal ActorAsset<Value> asset;
        private IActorContext<Value> customContext;
        public string CustomContextType { get; private set; }

        public void SetKey(string key)
        {
            this._key = key;
            SetDirty();
            ((ILocalizationActor)this).enable = true;
            ((ILocalizationActor)this).Execute();
        }

        public void SetContext(IActorContext<Value> context)
        {
            customContext = context;
            if (context != null)
            {
                CustomContextType = context.GetType().Name;
                SetMode(Mode.Custom);
                if (context is UnityEngine.ScriptableObject obj)
                {
                    SetMode(Mode.Asset);
                    asset = customContext as ActorAsset<Value>;
                }
            }
            SetDirty();
        }
        protected LocalizationMapActor(bool enable) : base(enable)
        {
            context.actor = this;
        }
        protected override void OnEditorLoad()
        {
            context.actor = this;
            base.OnEditorLoad();
        }
        internal bool AddLocalizationTypeToMap(string localizationType)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return false;
            return context.AddLocalizationTypeToMap(localizationType, GetDefault());
#endif
            return false;
        }
        internal sealed override void BeforeExecute(string localizationType)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                AddLocalizationTypeToMap(localizationType);
#endif
        }
        protected virtual Value GetDefault() => default;
        public Value GetValue() => GetValue(Localization.GetLocalizationType());
        public Value GetValue(string localizationType)
        {
            if (mode == Mode.Custom && customContext != null)
                return customContext.GetLocalization(localizationType, key);
            if (mode == Mode.Asset && asset != null)
                return asset.GetLocalization(localizationType, key);
            return ((IActorContext<Value>)context).GetLocalization(localizationType, key);
        }


    }
}
