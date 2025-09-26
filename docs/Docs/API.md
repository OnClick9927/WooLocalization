# Localization
``` csharp
// 设置语言偏好记录器
public static void SetRecorder(ILocalizationPrefRecorder recorder);

// 设置本地化数据源
public static void SetContext(LocalizationData context);

// 合并本地化数据源
public static void MergeContext(LocalizationData context);

// 设置默认语言
public static void SetDefaultLocalizationType(string type);

// 切换语言
public static void SetLocalizationType(string language);

// 获取当前语言
public static string GetLocalizationType();

// 获取所有支持的语言
public static List<string> GetLocalizationTypes();

// 基础文本本地化获取（支持格式化参数）
public static string GetLocalization(string key, params object[] args);

// 重载：指定数据源
public static string GetLocalization(LocalizationData context, string key, params object[] args);

// 重载：指定语言
public static string GetLocalization(string language, string key, params object[] args);

// 重载：指定数据源和语言
public static string GetLocalization(LocalizationData context, string language, string key, params object[] args);
```
# LocalizationEx
``` csharp
// UGUI Text组件本地化
public static void SetLocalization(this Text tmp, string key, params string[] args)

// TextMeshPro Text组件本地化
public static void SetLocalization(this TMP_Text tmp, string key, params string[] args)

// 通用组件本地化（指定行为类和演员类）
public static void SetLocalization<Behavior, Actor, Value>(this MonoBehaviour mono, IActorContext<Value> context, string key)
    where Behavior : LocalizationBehavior
    where Actor : LocalizationMapActor<Behavior, Value>

// 通过资源为组件设置本地化
public static void SetLocalizationByAsset<Behavior, Actor, Value>(this MonoBehaviour mono, ActorAsset<Value> context, string key)
    where Behavior : LocalizationBehavior
    where Actor : LocalizationMapActor<Behavior, Value>
```

# LocalizationText
``` csharp
// 文本内容本地化配置
public TextValueActor text;

// 字体本地化配置
public TextFontActor font;

// 字号本地化配置
public TextFontSizeActor fontSize;
```

# LocalizationTMP_Text
``` csharp
// 文本内容本地化配置
public TMPTextActor text;

// TMP字体本地化配置
public TMPFontActor font;

// TMP字号本地化配置
public TMPFontSizeActor fontSize;
```

# LocalizationData
``` csharp
// 合并另一个数据源
public void Merge(LocalizationData context);

// 获取指定语言的本地化内容
public string GetLocalization(string language, string key);

// 获取所有本地化键
public List<string> GetLocalizationKeys();

// 获取所有支持的语言
public List<string> GetLocalizationTypes();

public static void GenKeys();

// 拆分语言数据到新文件
public static void SplitLanguage<T>(ActorAsset<T> context, string language);

// 从Excel导入本地化数据
public static void ReadExcel();

// 导出为CSV
public static void WriteCSV();
```

# ActorAsset
``` csharp
// 获取所有本地化键
public List<string> GetLocalizationKeys();

// 获取所有支持的语言类型
public List<string> GetLocalizationTypes();

// 获取指定语言和键的本地化资源
public Value GetLocalization(string language, string key);

// 合并另一个数据源的指定语言数据
public void Merge(IActorContext<Value> context, string language);

// 合并另一个数据源的所有语言数据
public void Merge(IActorContext<Value> context);

// 添加或更新语言-键-值映射
public void AddPair(string language, string key, Value value);

// 清空指定语言的所有数据
public void ClearLanguage(string language);

// 清空指定键的所有语言数据
public void ClearKeys(IList<string> keys);

// 清空所有数据
public void Clear();

// 根据语言和值查找对应的键
public string FindKey(string language, Value val);
```

# LocalizationBehavior
``` csharp
// 本地化数据源（可在 Inspector 配置）
[SerializeField]
private LocalizationData _context;
internal LocalizationData context { get; set; }

// 默认本地化数据源（静态共享）
internal static LocalizationData defaultContext;

// 获取所有支持的语言类型（优先使用自身 context，否则使用全局数据）
public List<string> GetLocalizationTypes()

// 获取所有本地化键（优先使用自身 context，否则使用全局数据）
public List<string> GetLocalizationKeys()

// 获取本地化内容（优先使用自身 context，支持格式化参数）
public string GetLocalization(string key, params object[] args)
```