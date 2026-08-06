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
        }

        public abstract Value GetDefault();
        public Value GetValue() => GetValue(Localization.GetLocalizationType());
        public Value GetValue(string language)
        {
            if (mode == Mode.Custom && customContext != null)
                return customContext.GetLocalization(language, key);
            if (mode == Mode.Asset && asset != null)
                return asset.GetLocalization(language, key);

            Value v;
            if (map.TryGetValue(language, out v))
                return v;
#if UNITY_EDITOR
            if (mode == Mode.Default)
            {
                v = GetDefault();
                map.Add(language, v);
                return v;
            }
#endif
            return default;
        }
    }
}
