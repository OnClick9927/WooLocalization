/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-06-29
*********************************************************************************/


namespace WooLocalization
{
    [System.Serializable]
    class LocalizationEditorUserData : UnityEngine.ScriptableObject
    {

        [UnityEngine.SerializeField] private string _lastCSVPath = "Assets";
        [UnityEngine.SerializeField] private string _lastLocalizationDataPath = "Assets";
        public static string lastCSVPath
        {
            get { return context._lastCSVPath; }
            set
            {
                if (context._lastCSVPath != value)
                {
                    context._lastCSVPath = value;
                    context.Save();
                }

            }
        }
        public static string lastLocalizationDataPath
        {
            get { return context._lastLocalizationDataPath; }
            set
            {
                if (context._lastLocalizationDataPath != value)
                {
                    context._lastLocalizationDataPath = value;
                    context.Save();
                }

            }
        }
        private void Save()
        {
            LocalizationEditor.SaveContext(this);
        }
        private static LocalizationEditorUserData _context;
        private static LocalizationEditorUserData context
        {
            get
            {
                if (_context == null)
                {
                    _context = LocalizationEditor.LoadContext<LocalizationEditorUserData>("Assets/Editor/LocalizationEditorUserData.asset");
                }
                return _context;
            }
        }
    }
}
