/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-06-29
*********************************************************************************/

using System.IO;
using System.Linq;
using UnityEditor;

namespace WooLocalization
{
    [UnityEditor.InitializeOnLoad]
    class LocalizationEditor
    {
        private const string ObjDir = "Assets/Editor";

        private static string GetFilePath()
        {
            return AssetDatabase.GetAllAssetPaths().FirstOrDefault(x => x.Contains(nameof(WooLocalization))
            && x.EndsWith($"{nameof(LocalizationEditor)}.cs"));
        }
        public static string pkgPath
        {
            get
            {
                string packagePath = Path.GetFullPath("Packages/com.woo.localization");
                if (Directory.Exists(packagePath))
                {
                    return packagePath;
                }

                string path = GetFilePath();
                var index = path.LastIndexOf("WooLocalization");
                path = path.Substring(0, index + "WooLocalization".Length);
                return path;
            }
        }
        static LocalizationEditor()
        {
            if (!Directory.Exists(ObjDir))
                Directory.CreateDirectory(ObjDir);
            Localization.SetRecorder(context);

        }
        internal static T LoadContext<T>(string path) where T : UnityEngine.ScriptableObject
        {
            T _context;
            if (!System.IO.File.Exists(path))
            {
                _context = LocalizationSetting.CreateInstance<T>();
                AssetDatabase.CreateAsset(_context, path);
            }
            else
            {
                _context = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return _context;
        }
        internal static void SaveContext<T>(T context) where T : UnityEngine.ScriptableObject
        {
            EditorUtility.SetDirty(context);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static LocalizationSetting _context;
        internal static LocalizationSetting context
        {
            get
            {
                if (_context == null)
                {
                    _context = LoadContext<LocalizationSetting>("Assets/Editor/LocalizationSetting.asset");
                }
                return _context;
            }
        }
    }
}
