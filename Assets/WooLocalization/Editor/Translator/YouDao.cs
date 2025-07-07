/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace WooLocalization
{
    class YouDao : ITranslator
    {
        [Serializable]
        class Data
        {
            public string AppId = "";
            public string AppSecret = "";
        }
        public bool IsValid(string param)
        {

            var data = JsonUtility.FromJson<Data>(param);
            return data != null && !string.IsNullOrEmpty(data.AppId) && !string.IsNullOrEmpty(data.AppSecret);
        }

        public string OnGUI(string param)
        {
            var data = JsonUtility.FromJson<Data>(param);
            if (data == null)
                data = new Data();
            data.AppId = EditorGUILayout.TextField(nameof(data.AppId), data.AppId);
            data.AppSecret = EditorGUILayout.TextField(nameof(data.AppSecret), data.AppSecret);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Register"))
            {
                Application.OpenURL("https://ai.youdao.com/product-fanyi-text.s");
            }
            if (GUILayout.Button("Code && Language"))
            {
                Application.OpenURL("https://ai.youdao.com/DOCSIRMA/html/trans/api/plwbfy/index.html");
            }
            GUILayout.EndHorizontal();
            return JsonUtility.ToJson(data, false);
        }
        public async Task<TranslateResultCollection> Translate(string param, string[] content, string from, string to)
        {
            if (!IsValid(param))
                return null;
            var _data = JsonUtility.FromJson<Data>(param);

            string appKey = _data.AppId;
            string appSecret = _data.AppSecret;
            var dic = new Dictionary<string, string>();
            string[] qArray = content;
            string salt = DateTime.Now.Millisecond.ToString();
            dic.Add("from", from);
            dic.Add("to", to);
            dic.Add("signType", "v3");
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            long millis = (long)ts.TotalMilliseconds;
            string curtime = Convert.ToString(millis / 1000);
            dic.Add("curtime", curtime);
            string signStr = appKey + Truncate(string.Join("", qArray)) + salt + curtime + appSecret; ;
            string sign = ComputeHash(signStr, new SHA256CryptoServiceProvider());
            dic.Add("appKey", appKey);
            dic.Add("salt", salt);
            dic.Add("sign", sign);

            string url = "https://openapi.youdao.com/v2/api";
            int i = 0;
            foreach (var item in dic)
            {
                url += i == 0 ? "?" : "&";
                url += $"{item.Key}={item.Value}";
                i++;
            }
            foreach (var item in qArray)
                url += $"&q={UnityWebRequest.EscapeURL(item)}";
            var resuest = UnityWebRequest.Get(url);
            var op = resuest.SendWebRequest();

            while (!op.isDone)
                await Task.Delay(100);
            if (resuest.result == UnityWebRequest.Result.Success)
                return JsonUtility.FromJson<TranslateResultCollection>(resuest.downloadHandler.text);


            return null;

        }


        private string ComputeHash(string input, HashAlgorithm algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }
        private string Truncate(string q)
        {
            if (q == null)
            {
                return null;
            }
            int len = q.Length;
            return len <= 20 ? q : (q.Substring(0, 10) + len + q.Substring(len - 10, 10));
        }


    }
}
