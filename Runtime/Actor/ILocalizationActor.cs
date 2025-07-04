/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

namespace WooLocalization
{
    public interface ILocalizationActor
    {
        string name { get; }
        bool enable { get; set; }
        void Execute(string language, LocalizationBehavior component);
        void Execute();
        void SetBehavior(LocalizationBehavior behavior);
        void SetName(string name);
        void OnEditorLoad();
    }
}
