/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

namespace WooLocalization
{
    public abstract class LocalizationActor<T> : ILocalizationActor where T : LocalizationBehavior
    {
        [UnityEngine.SerializeField] private bool _enable = true;
        [UnityEngine.SerializeField] private string _name = string.Empty;
        [System.NonSerialized] private string _localizationType;
        [System.NonSerialized] private bool dirty = true;

        public void SetDirty()
        {
            dirty = true;
        }
        bool ILocalizationActor.enable
        {
            get => _enable; set
            {
                if (value != _enable)
                {
                    _enable = value;
                    if (value)
                    {
                        ((ILocalizationActor)this).Execute();
                    }
                }

            }
        }
        protected LocalizationActor(bool enable)
        {
            this._enable = enable;
            SetDirty();
        }
        protected T behavior { get; private set; }

        string ILocalizationActor.name => _name;



        public LocalizationActor<T> SetName(string name)
        {
            _name = name;
            return this;
        }
        void ILocalizationActor.SetName(string name)
        {
            _name = name;
        }
        void ILocalizationActor.Execute()
        {
            var language = Localization.GetLocalizationType();
            ((ILocalizationActor)this).Execute(language, behavior);
        }
        void ILocalizationActor.SetBehavior(LocalizationBehavior behavior)
        {
            this.behavior = behavior as T;


        }
        protected bool NeedExecute(string language)
        {
            if (!_enable) return false;
            if (_localizationType != language) return true;
            return dirty;
        }
        void ILocalizationActor.Execute(string language, LocalizationBehavior component)
        {
            if (!NeedExecute(language)) return;
            T targetBehavior = component as T;
            if (targetBehavior == null) return;
            dirty = false;
            _localizationType = language;
            Execute(language, targetBehavior);
        }
        protected virtual void OnEditorLoad() { }
        protected abstract void Execute(string language, T component);
        //internal virtual void BeforeExecute(string language) { }

        void ILocalizationActor.OnEditorLoad() => OnEditorLoad();

  
    }
}
