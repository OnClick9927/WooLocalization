using WooLocalization;
using UnityEngine;

public class LocalizationGame : MonoBehaviour, ILocalizationPrefRecorder
{

    public TMPro.TextMeshProUGUI text;
    [ContextMenu("HH")]
    public void GG()
    {
        text.gameObject.AddComponent<LocalizationTMP_Text>();
    }

    public LocalizationData data;
    private void Start()
    {
        Localization.SetContext(data);
        Localization.SetRecorder(this);
    }

    private void OnGUI()
    {
        var types = Localization.GetLocalizationTypes();
        var type = Localization.localizationType;
        var index = GUILayout.Toolbar(Mathf.Max(types.IndexOf(type), 0), types.ToArray(), new GUIStyle(GUI.skin.button) { fontSize = 40 }, GUILayout.Height(100), GUILayout.Width(300));
        Localization.SetLocalizationType(types[index]);
    }

    LocalizationPref ILocalizationPrefRecorder.Read()
    {
        Debug.Log("Read");

        return null;
    }

    void ILocalizationPrefRecorder.Write(LocalizationPref pref)
    {
        Debug.Log("Write");
    }
}
