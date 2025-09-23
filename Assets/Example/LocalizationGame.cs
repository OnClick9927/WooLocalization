using WooLocalization;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationGame : MonoBehaviour, ILocalizationPrefRecorder
{
    public SpriteActorAsset sprite;
    public Image image;
    public TMPro.TextMeshProUGUI text;

    public LocalizationData data;
    [ContextMenu("HHHH")]
    public void Test()
    {
        text.SetLocalization(LocalizationKeys.String.Load_Tip_Text);

    }
    private void Start()
    {
        Localization.SetRecorder(this);
        Localization.SetContext(data);
        Localization.SetDefaultLocalizationType(Languages.zh_CHS);
        text.SetLocalization(LocalizationKeys.String.Load_Tip_Text);
        image.SetLocalizationByAsset<LocalizationImage, LocalizationImage.ImageSpriteActor,Sprite>(sprite, LocalizationKeys.Sprite.text);
    }

    private void OnGUI()
    {
        var types = Localization.GetLocalizationTypes();
        var type = Localization.language;
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
