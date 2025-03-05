/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
namespace WooLocalization
{
    public interface IMapActorContext<Value>
    {
        Value GetValue(string localizationType, string key);
    }
}
