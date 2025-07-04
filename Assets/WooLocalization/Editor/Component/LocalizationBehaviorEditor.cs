/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WooLocalization
{
    public abstract class LocalizationBehaviorEditor<T> : UnityEditor.Editor where T : LocalizationBehavior
    {


        protected T comp { get; private set; }

        private void Create(List<Type> FieldTypes, List<ILocalizationActor> actors, Dictionary<Type, ILocalizationActorEditor> insMap, Dictionary<Type, ILocalizationActorEditor> insMap_obj)
        {
            for (int i = 0; i < FieldTypes.Count; i++)
            {
                var actor = actors[i];
                var FieldType = FieldTypes[i];
                Type type;

                if (LocalizationEditorHelper.ExistActorEditor(FieldType, out type))
                {
                    if (!insMap.ContainsKey(type))
                        insMap[type] = LocalizationEditorHelper.CreateEditor(type);
                    var editor = insMap[type];
                    var value = actor;
                    AddField(actor.name, editor, value);
                }
                else
                {
                    if (FieldType.IsGenericType)
                    {
                        var types = FieldType.GetGenericArguments();
                        var type0 = types[0];
                        if (typeof(ObjectActor<>).MakeGenericType(type0) == FieldType)
                        {
                            if (!insMap_obj.ContainsKey(type0))
                            {
                                var genericType = typeof(ObjectActorEditor<>).MakeGenericType(new Type[] { type0 });
                                insMap_obj.Add(type0, LocalizationEditorHelper.CreateEditor(genericType));
                            }
                            var editor = insMap_obj[type0];
                            var value = actor;
                            AddField(actor.name, editor, value);
                        }
                    }
                }
            }

        }
        protected void LoadFields()
        {
            fields.Clear();
            var fieldInfos = comp.GetType().GetFields().Where(x => typeof(ILocalizationActor).IsAssignableFrom(x.FieldType)).ToList()
                   .ToList();
            var insMap = new Dictionary<Type, ILocalizationActorEditor>();
            var insMap_obj = new Dictionary<Type, ILocalizationActorEditor>();

            Create(fieldInfos.Select(x => x.FieldType).ToList(), fieldInfos.Select(x =>
            {
                var actor = x.GetValue(comp) as ILocalizationActor;
                actor.SetName(x.Name);
                return actor;
            }).ToList(), insMap, insMap_obj);

            var actors = new List<ILocalizationActor>(comp.LoadActors());
            actors.RemoveAll(a => fields.Any(x => x.value == a));
            Create(actors.Select(x => x.GetType()).ToList(), actors, insMap, insMap_obj);
        }
        private void Awake()
        {
            comp = target as T;
            if (comp.context == null && LocalizationSetting.defaultData != null)
            {
                comp.context = LocalizationSetting.defaultData;
                EditorUtility.SetDirty(comp);
            }
            LoadFields();
            comp.enabled = false;
            EditorApplication.delayCall += () =>
            {
                comp.enabled = true;
            };

        }
        private void OnEnable() => LoadFields();

        private void AddField(string name, ILocalizationActorEditor editor, object value)
        {
            fields.Add(new Field() { editor = editor, name = name, value = value });
        }
        private List<Field> fields = new List<Field>();

        private class Field
        {
            public string name;
            public object value;
            public ILocalizationActorEditor editor;
        }

        protected void DrawFields()
        {
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];

                field.editor.OnGUI(field.name, comp, field.value);


                AssetDatabase.SaveAssetIfDirty(comp);

            }
        }
        protected void DrawContext()
        {
            comp.context = EditorGUILayout.ObjectField(nameof(LocalizationBehavior.context),
      comp.context, typeof(LocalizationData), false) as LocalizationData;
            EditorGUILayout.LabelField(nameof(Localization), Localization.GetLocalizationType());

        }
        protected virtual void DrawInspectorGUI()
        {
            DrawContext();
            DrawFields();
        }
        public sealed override void OnInspectorGUI()
        {
            GUI.enabled = !Application.isPlaying;
            DrawInspectorGUI();
            GUI.enabled = true;

        }
    }
}
