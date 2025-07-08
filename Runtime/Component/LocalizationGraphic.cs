/*********************************************************************************
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
    public class LocalizationGraphic : LocalizationBehavior<UnityEngine.UI.Graphic>
    {
        [System.Serializable]
        public class GraphicColorActor : LocalizationMapActor<LocalizationGraphic, Color>
        {
            public GraphicColorActor(bool enable) : base(enable)
            {
            }


            public override Color GetDefault()
            {
                if (behavior != null)
                    return behavior.target.color;
                return Color.white;
            }

            protected override void Execute(string language, LocalizationGraphic component)
            {
                component.target.color = GetValue(language);

            }
        }
        [System.Serializable]
        public class GraphicMaterialActor : LocalizationMapActor<LocalizationGraphic, Material>
        {
          
            public GraphicMaterialActor(bool enable) : base(enable)
            {
            }
            public override Material GetDefault()
            {
                if (behavior != null)
                    return behavior.target.material;
                return UnityEngine.UI.Graphic.defaultGraphicMaterial;
            }

            protected override void Execute(string language, LocalizationGraphic component)
            {
                component.target.material = GetValue(language);

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
        public T graphic
        {

            get
            {

                if (_graphic == null)
                {
                    _graphic = target as T;
                }
                return _graphic;
            }
        }
    }
}
