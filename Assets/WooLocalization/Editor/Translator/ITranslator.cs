/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Threading.Tasks;

namespace WooLocalization
{
    public interface ITranslator
    {
        Task<TranslateResultCollection> Translate(string param, string[] content, string from, string to);
        bool IsValid(string param);

        string OnGUI(string param);
    }
}
