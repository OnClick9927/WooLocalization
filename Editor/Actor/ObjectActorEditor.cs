/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
namespace WooLocalization
{
    [LocalizationActorEditorAttribute]

    class ObjectActorEditor<V> : LocalizationMapActorEditor<ObjectActor<V>, V, LocalizationBehavior>
    {
        protected override bool NeedExecute()
        {
            return false;
        }

    }
}
