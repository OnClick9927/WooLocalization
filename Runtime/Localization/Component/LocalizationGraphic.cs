﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WooLocalization
{
    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Graphic))]
    [DisallowMultipleComponent]
    public class LocalizationGraphic : LocalizationBehavior
    {
        [System.Serializable]
        public class GraphicColorActor : LocalizationMapActor<LocalizationGraphic, Color>
        {
            public GraphicColorActor(bool enable) : base(enable)
            {
            }

            public override Color GetDefault()
            {
                return Color.white;
            }

            protected override void Execute(string localizationType, LocalizationGraphic component)
            {
                component.graphic.color = GetValue(localizationType);

            }
        }
        [System.Serializable]
        public class GraphicMaterialActor : LocalizationMapActor<LocalizationGraphic, Material>
        {
            public SerializableDictionary<string, Material> materials = new SerializableDictionary<string, Material>();

            public GraphicMaterialActor(bool enable) : base(enable)
            {
            }

            protected override void Execute(string localizationType, LocalizationGraphic component)
            {
                component.graphic.material = GetValue(localizationType);

            }

            public override Material GetDefault()
            {
                return UnityEngine.UI.Graphic.defaultGraphicMaterial;
            }
        }
        [NonSerialized] private Graphic _graphic; 
        public Graphic graphic
        {
            get
            {
                if (_graphic == null)
                {
                    _graphic = GetComponent<Graphic>();
                }
                return _graphic;
            }
        }
        public GraphicColorActor color = new GraphicColorActor(false);
        public GraphicMaterialActor material = new GraphicMaterialActor(false);


        protected override List<ILocalizationActor> GetActors()
        {
            return new List<ILocalizationActor>() {
                color,material,
           };
        }
    }

    public class LocalizationGraphic<T> : LocalizationGraphic where T : Graphic
    {
        [NonSerialized] private T _graphic;
        public T graphicT
        {

            get
            {

                if (_graphic == null)
                {
                    _graphic = graphic as T;
                }
                return _graphic;
            }
        }
    }
}
