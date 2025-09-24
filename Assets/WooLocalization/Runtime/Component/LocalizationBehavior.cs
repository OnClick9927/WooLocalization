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
    public abstract class LocalizationBehavior<T> : LocalizationBehavior
    {
        private T _target;
        public T target
        {
            get
            {
                if (_target == null)
                {
                    _target = GetComponent<T>();
                }
                return _target;
            }
        }
    }
    [ExecuteAlways]
    public abstract class LocalizationBehavior : MonoBehaviour, ILocalizationEventBehavior
    {
        [SerializeField]
        private LocalizationData _context;
        internal LocalizationData context { get { return _context; } set { _context = value; } }

        internal static LocalizationData defaultContext;



        public List<string> GetLocalizationTypes() => context == null ? Localization.GetLocalizationTypes() : Localization.GetLocalizationTypes(_context);
        public List<string> GetLocalizationKeys() => context == null ? Localization.GetLocalizationKeys() : Localization.GetLocalizationKeys(_context);
        public string GetLocalization(string key, params object[] args) => context == null ? Localization.GetLocalization(key, args) : Localization.GetLocalization(_context, key, args);




        protected List<ILocalizationActor> actors { get; private set; }

        public void EnableAll(bool enable)
        {
            var actors = LoadActors();
            for (int i = 0; i < actors.Count; i++)
            {
                actors[i].enable = enable;
            }
        }

        public string FindKey(string type, string value)
        {
            if (_context != null)
            {
                var key = _context.FindKey(type, value);
                return key;
            }
            return string.Empty;
        }

        public T FindActor<T>() where T : class, ILocalizationActor
        {
            var actors = LoadActors();
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i] is T) return actors[i] as T;
            }
            return default;
        }
        public List<ILocalizationActor> LoadActors()
        {
            var _actors = actors;
            if (_actors != null) return _actors;
            _actors = GetActors();

            for (int i = 0; i < _actors.Count; i++)
            {
                var actor = _actors[i];
                actor.SetBehavior(this);
            }
  
            actors = _actors;
            return _actors;
        }


        protected virtual void Awake()
        {


            if (Application.isPlaying)
            {
                if (_context == null)
                    _context = Localization.context;
            }
            else
            {
#if UNITY_EDITOR
                if (_context == null)
                    _context = defaultContext;
#endif
            }
            LoadActors();
        }




        protected void OnDisable()
        {
            Localization.RemoveBehavior(this);
        }
        protected void OnEnable()
        {
            Localization.AddBehavior(this);
# if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += Execute;
#else
            Execute();
#endif
        }

        protected abstract List<ILocalizationActor> GetActors();

        void ILocalizationEventBehavior.OnLanguageChange() => Execute();

        private void Execute()
        {
            LoadActors();
            var _type = Localization.GetLocalizationType();
            var actors = LoadActors();
            for (int i = 0; i < actors.Count; i++)
                actors[i].Execute(_type, this);
        }

        void ILocalizationEventBehavior.SetActorsDirty()
        {
            var actors = LoadActors();
            for (int i = 0; i < actors.Count; i++)
                actors[i].SetDirty();
        }
    }
}
