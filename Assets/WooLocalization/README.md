## upm 添加包地址 https://github.com/OnClick9927/WooLocalization.git#upm

[![Stargazers over time](https://starchart.cc/OnClick9927/WooLocalization.svg?variant=adaptive)](https://starchart.cc/OnClick9927/WooLocalization)

# WooLocalization [Doc](https://onclick9927.github.io/WooLocalization)

*  支持  文本/图片/特效/预制体/继承自UnityEngine.Object 本地化
*  提供本地化组件（支持挖孔的文本，支持多种属性变化）
*  支持一键翻译 \自定义翻译器（内置有道）
*  支持无限多语言类型
*  支持扩展自定义本地化
*  支持导入导出自定义本地化文件 （Excel、CSV）
*  支持编辑器增加/检查/删除/修改   本地化 Key/语言
*  支持编辑器下预览语言切换
*  支持   任意组件 本地化
*  支持 本地化 分文件/分布加载（比如只需要中文的时候，不加载英文）
*  支持生成 本地化的 key

~~~ csharp
public class LocalizationGame : MonoBehaviour, ILocalizationPrefRecorder
{
    public SpriteActorAsset sprite;
    public Image image;
    public TMPro.TextMeshProUGUI text;

    public LocalizationData data;
    private void Start()
    {
        // 初始化三步骤
        // 设置记录器具，主要是如何记录当前的语言
        Localization.SetRecorder(this);
        // 设置提供 本地化 资源
        Localization.SetContext(data);
        // 设置 默认的语言
        Localization.SetDefaultLocalizationType(LocalizationKeys.Languages.zh_Hans);


        //给组件设置 本地化
        text.SetLocalization(LocalizationKeys.String.Load_Tip_Text);
        image.SetLocalizationByAsset<LocalizationImage, LocalizationImage.ImageSpriteActor,Sprite>(sprite, LocalizationKeys.Sprite.text);

// 非组件 部分 获取 本地化
        Debug.Log(Localization.GetLocalization(LocalizationKeys.String.Load_Tip_Text));
    // 合并 本地化，实现 不用语种非同时加载
      //  Localization.MergeContext(data);

    }
///动态切换语言
    private void OnGUI()
    {
        var types = Localization.GetLocalizationTypes();
        var type = Localization.localizationType;
        var index = GUILayout.Toolbar(Mathf.Max(types.IndexOf(type), 0), types.ToArray(), new GUIStyle(GUI.skin.button) { fontSize = 40 }, GUILayout.Height(100), GUILayout.Width(300));
        Localization.SetLocalizationType(types[index]);
    }

// 本地化记录回调
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

~~~


![image](https://github.com/user-attachments/assets/3f774100-1669-417e-9607-dc233fda72c5)
<img width="382" alt="{2C20A8AB-E22C-43F7-98A2-779A58D24CA1}" src="https://github.com/user-attachments/assets/0c67feb4-432f-486f-ac47-52f23944c856" />
![f489f59c-8f8b-4fe1-b7c2-7835a6f36eb5](https://github.com/user-attachments/assets/d6f83a09-2015-45fd-83e2-80467d9ebdba)
![1e264983-dcd4-4a91-9659-485ad277ba71](https://github.com/user-attachments/assets/f735af84-b2bc-416f-bf6a-3a3bc0eb3e06)
![7541e788-84ce-4fb8-a90c-d724ff66a629](https://github.com/user-attachments/assets/8ae4c94f-2391-456e-9a3d-659ff73e9a18)
![665ff1bf-3c76-4dc1-b8e8-90c602a38bb4](https://github.com/user-attachments/assets/62e37823-9180-4782-9a8a-58fce72d8c0a)
![image](https://github.com/user-attachments/assets/2f46dd79-3e4d-4238-ae17-48ff0ec1a193)



### 引用 [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader) 



