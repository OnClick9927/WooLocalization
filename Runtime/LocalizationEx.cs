/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace WooLocalization
{
    public static class LocalizationEx
    {
        public static void SetLocalization(this Text tmp, string key, params string[] args)
        {
            var comp = tmp.GetComponent<LocalizationText>();
            if (comp == null)
                comp = tmp.gameObject.AddComponent<LocalizationText>();
            comp.LoadActors();
            comp.text.formatArgs = args;
            comp.text.SetKey(key);
        }

        public static void SetLocalization<Behavior, Actor, Value>(this MonoBehaviour mono, IActorContext<Value> context, string key)
            where Behavior : LocalizationBehavior
            where Actor : LocalizationMapActor<Behavior, Value>
        {
            var comp = mono.GetComponent<Behavior>();
            if (comp == null)
                comp = mono.gameObject.AddComponent<Behavior>();
            var actor = comp.FindActor<Actor>();
            actor.SetContext(context);
            actor.SetKey(key);
        }
        public static void SetLocalizationByAsset<Behavior, Actor, Value>(this MonoBehaviour mono, ActorAsset<Value> context, string key)
                where Behavior : LocalizationBehavior
                where Actor : LocalizationMapActor<Behavior, Value>
            => SetLocalization<Behavior, Actor, Value>(mono, context, key);
    }
}
