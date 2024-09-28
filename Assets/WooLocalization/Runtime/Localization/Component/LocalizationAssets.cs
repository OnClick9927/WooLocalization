/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using System.Collections.Generic;
namespace WooLocalization
{

    public class LocalizationAssets<T> : LocalizationBehavior
    {

        public List<ObjectActor<T>> objects = new List<ObjectActor<T>>();
        private Dictionary<string, ObjectActor<T>> objectMap = new Dictionary<string, ObjectActor<T>>();
        public T GetObject(string key)
        {
            if (!objectMap.ContainsKey(key)) return default;
            var actor = objectMap[key];
            return actor.GetValue();
        }
        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                objectMap.Add((obj as ILocalizationActor).name, obj);
            }
        }
        protected override List<ILocalizationActor> GetActors()
        {
            var list = new List<ILocalizationActor>();
            list.AddRange(objects);

            return list;
        }
    }
}
