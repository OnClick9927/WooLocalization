/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WooLocalization
{
    public static class LocalizationEditorHelper
    {
        class CSVHelper
        {
            private static string FieldsToLine(IEnumerable<string> fields)
            {
                if (fields == null) return string.Empty;
                fields = fields.Select(field =>
                {
                    if (field == null) field = string.Empty;
                    return field;
                });
                string line = string.Format("{0}{1}", string.Join(",", fields), Environment.NewLine);
                return line;
            }
            public static void Write(string path, List<string[]> fields)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < fields.Count; i++)
                {
                    sb.Append(FieldsToLine(fields[i]));
                }
                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            }


            static int index = 0;
            static string txt;
            static StringBuilder sbr;
            static Regex lineReg;
            static Regex fieldReg;
            static Regex quotesReg;
            public static void BeginRead(string path)
            {
                lineReg = new Regex(LocalizationSetting.lineReg);
                fieldReg = new Regex(LocalizationSetting.fieldReg);
                quotesReg = new Regex(LocalizationSetting.quotesReg);
                sbr = new StringBuilder();
                index = 0;
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Dispose();
                txt = Encoding.UTF8.GetString(bytes);
            }

            public static string[] ReadFields()
            {
                while (true)
                {
                    var line = ReadLine(txt, ref index);
                    if (line == null) break;
                    sbr.Append(line);
                    //一个完整的CSV记录行，它的双引号一定是偶数
                    if (lineReg.Matches(sbr.ToString()).Count % 2 == 0)
                    {
                        var fields = ParseCsvLine(sbr.ToString(), fieldReg, quotesReg).ToArray();
                        sbr.Clear();
                        return fields;
                    }
                    else
                        sbr.Append(Environment.NewLine);
                }
                return null;
            }
            private static string ReadLine(string txt, ref int index)
            {
                StringBuilder sbr = new StringBuilder();
                char c;
                while (txt.Length > index)
                {
                    c = (char)txt[index];
                    index++;
                    if (c == '\n' && sbr.Length > 0 && sbr[sbr.Length - 1] == '\r')
                    {
                        sbr.Remove(sbr.Length - 1, 1);
                        return sbr.ToString();
                    }
                    else
                    {
                        sbr.Append(c);
                    }
                }
                return sbr.Length > 0 ? sbr.ToString() : null;
            }
            private static List<string> ParseCsvLine(string line, Regex fieldReg, Regex quotesReg)
            {
                var fieldMath = fieldReg.Match(line);
                List<string> fields = new List<string>();
                while (fieldMath.Success)
                {
                    string field;
                    if (fieldMath.Groups[1].Success)
                    {
                        field = quotesReg.Replace(fieldMath.Groups[1].Value, "\"");
                    }
                    else
                    {
                        field = fieldMath.Groups[2].Value;
                    }
                    fields.Add(field);
                    fieldMath = fieldMath.NextMatch();
                }
                return fields;
            }
        }

        class YouDao
        {


            [System.Serializable]
            public class TranslateResult
            {
                public int errorCode;
                public string[] translation;
                public string speakUrl;
                public string requestId;
                public string tSpeakUrl;
                public string l;
                public bool isWord;
                public string query;
            }
            public static async Task<TranslateResult> Translate(string content, string from, string to)
            {
                string appKey = LocalizationSetting.youDaoAppId;
                string appSecret = LocalizationSetting.youDaoAppSecret;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://openapi.youdao.com/api");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                //当前UTC时间戳（秒）
                string curtime = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000).ToString();
                //UUID 唯一通用识别码
                string salt = DateTime.Now.Millisecond.ToString();
                string input = content == null ? null : content.Length <= 20 ? content : (content.Substring(0, 10) + content.Length + content.Substring(content.Length - 10, 10));
                byte[] inputBytes = Encoding.UTF8.GetBytes(appKey + input + salt + curtime + appSecret);
                byte[] hashedBytes = new SHA256CryptoServiceProvider().ComputeHash(inputBytes);
                //签名 sha256(应用ID + input + salt + curtime + 应用秘钥)
                //其中input的计算方式为：input=content前10个字符 + content长度 + cotent后10个字符（当cotent长度大于20）或 input=content字符串（当content长度小于等于20）
                string sign = BitConverter.ToString(hashedBytes).Replace("-", "");
                //签名类型
                string signType = "v3";
                //参数列表
                string args = string.Format("from={0}&to={1}&signType={2}&curtime={3}&q={4}&appKey={5}&salt={6}&sign={7}",
                    from, to, signType, curtime, content, appKey, salt, sign);
                byte[] data = Encoding.UTF8.GetBytes(args);
                request.ContentLength = data.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    await reqStream.WriteAsync(data, 0, data.Length);
                    reqStream.Close();
                }
                var httpWebResponse = await request.GetResponseAsync();
                Stream stream = httpWebResponse.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string responseStr = await reader.ReadToEndAsync();

                    var response = JsonUtility.FromJson<TranslateResult>(responseStr);
                    return response;
                }
            }

        }

        public static V DrawObject<V>(string lan, V value)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(V)))
                return (V)(object)EditorGUILayout.ObjectField(lan, (UnityEngine.Object)(object)value, typeof(V), false);

            else if (typeof(V) == typeof(string))
            {
                string src = string.Empty;
                if (value != null)
                    src = (string)(object)value;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(lan, GUILayout.Width(100));
                string tmp = EditorGUILayout.TextArea(src, GUILayout.MinHeight(40));
                GUILayout.EndHorizontal();
                return (V)(object)tmp;
            }
            else if (typeof(V) == typeof(int))
                return (V)(object)EditorGUILayout.IntField(lan, (int)(object)value);
            else if (typeof(V) == typeof(float))
                return (V)(object)EditorGUILayout.FloatField(lan, (float)(object)value);
            else if (typeof(V) == typeof(bool))
                return (V)(object)EditorGUILayout.Toggle(lan, (bool)(object)value);
            else if (typeof(V) == typeof(double))
                return (V)(object)EditorGUILayout.DoubleField(lan, (double)(object)value);
            else if (typeof(V) == typeof(long))
                return (V)(object)EditorGUILayout.LongField(lan, (long)(object)value);
            else if (typeof(V) == typeof(Color))
                return (V)(object)EditorGUILayout.ColorField(lan, (Color)(object)value);
            else if (typeof(V) == typeof(Vector3))
                return (V)(object)EditorGUILayout.Vector3Field(lan, (Vector3)(object)value);
            else if (typeof(V) == typeof(Vector2))
                return (V)(object)EditorGUILayout.Vector2Field(lan, (Vector2)(object)value);
            else if (typeof(V) == typeof(Vector4))
                return (V)(object)EditorGUILayout.Vector4Field(lan, (Vector4)(object)value);
            else if (typeof(V) == typeof(Vector2Int))
                return (V)(object)EditorGUILayout.Vector2IntField(lan, (Vector2Int)(object)value);
            else if (typeof(V) == typeof(Vector3Int))
                return (V)(object)EditorGUILayout.Vector3IntField(lan, (Vector3Int)(object)value);
            else if (typeof(V) == typeof(Rect))
                return (V)(object)EditorGUILayout.RectField(lan, (Rect)(object)value);
            else if (typeof(V) == typeof(RectInt))
                return (V)(object)EditorGUILayout.RectIntField(lan, (RectInt)(object)value);
            else if (typeof(V) == typeof(Bounds))
                return (V)(object)EditorGUILayout.BoundsField(lan, (Bounds)(object)value);
            else if (typeof(V) == typeof(AnimationCurve))
            {
                AnimationCurve src = (AnimationCurve)(object)value;
                if (src == null) src = new AnimationCurve();
                return (V)(object)EditorGUILayout.CurveField(lan, src);
            }


            return default;
        }

        public static void SaveContext(LocalizationData context)
        {
            EditorUtility.SetDirty(context);
            AssetDatabase.SaveAssetIfDirty(context);
        }

        public static void ReadCSV(string path, LocalizationData context)
        {
            CSVHelper.BeginRead(path);
            int index = 0;
            string[] lanTypes = null;
            while (true)
            {
                var fields = CSVHelper.ReadFields();
                if (fields == null) break;
                if (index == 0)
                {
                    lanTypes = fields;
                }
                else
                {
                    var key = fields[0];
                    for (int j = 1; j < fields.Length; j++)
                    {
                        var value = fields[j];
                        var lan = lanTypes[j];
                        context.Add(lan, key, value);
                    }
                }

                index++;
            }
            SaveContext(context);
        }
        private static string WrapInQuotes(string value)
        {
            //如果为空，则返回空字符串
            if (string.IsNullOrEmpty(value)) return string.Empty;

            //如果已经有引号，则进行转义
            value = value.Replace("\"", "\"\"");

            return $"\"{value}\""; //转义后加上双引号
        }

        public static void WriteCSV(string path, LocalizationData context)
        {

            if (string.IsNullOrEmpty(path)) return;
            var types = context.GetLocalizationTypes();
            var keys = context.GetLocalizationKeys();
            var header = new List<string>(types);
            header.Insert(0, "Key");
            List<string[]> result = new List<string[]>() { header.ToArray() };

            for (int i = 0; i < keys.Count; i++)
            {
                string[] _content = new string[types.Count + 1];
                var key = keys[i];
                _content[0] = WrapInQuotes(key); //转义
                for (int j = 0; j < types.Count; j++)
                {
                    var type = types[j];
                    var value = context.GetLocalization(type, key);
                    _content[j + 1] = WrapInQuotes(value); //转义
                }
                result.Add(_content);
            }

            CSVHelper.Write(path, result);
        }


        public static void ClearContext(LocalizationData context)
        {
            context.Clear();
            SaveContext(context);
        }
        public static string ToAssetsPath(string self)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            return "Assets" + Path.GetFullPath(self).Substring(assetRootPath.Length).Replace("\\", "/");
        }
        public static void ReadLocalizationData(LocalizationData src, LocalizationData context)
        {
            if (src == null) return;
            var types = src.GetLocalizationTypes();
            var keys = src.GetLocalizationKeys();
            foreach (var key in keys)
            {
                foreach (var type in types)
                {
                    var value = src.GetLocalization(type, key);
                    context.Add(type, key, value);
                }
            }
            SaveContext(context);
        }

        public static async Task Translate(LocalizationData context, List<string> keys, string src, string dest)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                var from = context.GetLocalization(src, key);
                var result = await YouDao.Translate(from,
                    LocalizationSetting.GetLocalizationTypeReflect(src), LocalizationSetting.GetLocalizationTypeReflect(dest));
                if (result.errorCode == 0)
                {
                    var value = result.translation[0];
                    context.Add(dest, key, value);
                }
                else
                {
                    Debug.LogError($"key:{key}\t from:{from}\t ErrCode:{result.errorCode}");
                }
            }
            EditorApplication.delayCall += () =>
            {
                SaveContext(context);
            };
        }

        public static void DeleteKeys(LocalizationData context, List<string> keys)
        {
            context.ClearKeys(keys);

            SaveContext(context);

        }
        public static void DeleteLocalizationType(LocalizationData context, string type)
        {
            context.ClearLan(type);


            SaveContext(context);

        }
        public static void AddLocalizationType(LocalizationData context, string type)
        {
            context.Add(type);
            SaveContext(context);

        }
        public static void AddLocalizationPair(LocalizationData context, string type, string key, string val)
        {
            context.Add(type, key, val);
            SaveContext(context);

        }


    }
}
