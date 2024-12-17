﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace WooLocalization
{

    [ExecuteAlways]
    public abstract class LocalizationBehavior : MonoBehaviour, ILocalizationEventActor
    {
        [SerializeField]
        private LocalizationData _context;
        public LocalizationData context { get { return _context; } set { _context = value; } }

        public static LocalizationData defaultContext;
        public List<string> GetLocalizationTypes()
        {
            if (_context == null)
                return Localization.GetLocalizationTypes();
            return Localization.GetLocalizationTypes(_context);
        }
        public List<string> GetLocalizationKeys()
        {
            if (_context == null)
                return Localization.GetLocalizationKeys();
            return Localization.GetLocalizationKeys(_context);
        }
        public string GetLocalization(string key)
        {
            if (_context == null)
                return Localization.GetLocalization(key);
            return Localization.GetLocalization(_context, key);
        }
        public string GetLocalization(string type, string key)
        {
            if (_context == null)
                return Localization.GetLocalization(type, key);
            return Localization.GetLocalization(_context, type, key);
        }
        public string GetLocalizationType()
        {
            return Localization.GetLocalizationType();
        }

        protected List<ILocalizationActor> actors { get; private set; }

        public void EnableAll(bool enable)
        {
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

        public List<ILocalizationActor> LoadActors()
        {
            actors = GetActors();

            for (int i = 0; i < actors.Count; i++)
            {
                var actor = actors[i];
                actor.SetBehavior(this);
            }
#if UNITY_EDITOR
            OnAddComponent();
#endif
            return actors;
        }

        private void OnAddComponent()
        {
            for (int i = 0; i < actors.Count; i++)
            {
                var actor = actors[i];
                actor.OnAddComponent();
            }
        }

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (_context == null)
                _context = defaultContext;
#endif
            LoadActors();
        }




        protected void OnDisable()
        {
            Localization.RemoveHandler(this);
        }
        protected void OnEnable()
        {
            Localization.AddHandler(this);
            Execute();
        }

        protected abstract List<ILocalizationActor> GetActors();

        public void AddActor(ILocalizationActor actor)
        {
            if (actors.Contains(actor)) return;
            actors.Add(actor);
            actor.SetBehavior(this);
            actor.Execute(Localization.GetLocalizationType(), this);
        }
        public void RemoveActor(ILocalizationActor actor)
        {
            actors.Remove(actor);
        }
        void ILocalizationEventActor.OnLocalizationChange()
        {
            Execute();
        }
        private void Execute()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                LoadActors();
            }
#endif
            var _type = Localization.GetLocalizationType();
            for (int i = 0; i < actors.Count; i++)
                actors[i].Execute(_type, this);
        }


    }
}