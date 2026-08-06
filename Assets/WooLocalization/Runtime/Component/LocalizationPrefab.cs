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
    [DisallowMultipleComponent]
    public class LocalizationPrefab : LocalizationBehavior
    {
        [System.Serializable]

        public class PrefabActor : LocalizationMapActor<LocalizationBehavior, GameObject>
        {
            private GameObject ins;
            private GameObject currentPrefab;
            private string currentLanguage;

            public PrefabActor(bool enable) : base(enable)
            {
            }
            public override GameObject GetDefault() => default;

            protected override void Execute(string language, LocalizationBehavior component)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) return;

                if (UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(component)) return;
#endif
                var prefab = GetValue(language);
                if (ReferenceEquals(prefab, currentPrefab) && currentLanguage != language && (prefab == null || ins != null))
                {
                    currentLanguage = language;
                    return;
                }

                currentPrefab = prefab;
                currentLanguage = language;
                if (ins != null)
                {
                    GameObject.Destroy(ins);
                    ins = null;
                }

                if (prefab != null)
                    ins = GameObject.Instantiate(prefab, component.transform);

            }

            public void Create()
            {
                ((ILocalizationActor)this).enable = true;
                ((ILocalizationActor)this).Execute();
            }


        }
        public PrefabActor prefab = new PrefabActor(true);
        protected override List<ILocalizationActor> GetActors()
        {
            return new List<ILocalizationActor>(1) { prefab };
        }
    }
}
