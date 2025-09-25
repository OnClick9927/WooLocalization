/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace WooLocalization
{


    [UnityEditor.InitializeOnLoad]
    public static class LocalizationEditorHelper
    {

        private const string ObjDir = "Assets/Editor";

        private static string GetFilePath()
        {
            return AssetDatabase.GetAllAssetPaths().FirstOrDefault(x => x.Contains(nameof(WooLocalization))
            && x.EndsWith($"{nameof(LocalizationEditorHelper)}.cs"));
        }
        internal static string pkgPath
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
        private static Dictionary<Type, Type> actorEditors = new Dictionary<Type, Type>();
        public static bool ExistActorEditor(Type type, out Type result) => actorEditors.TryGetValue(type, out result);
        internal static ILocalizationActorEditor CreateEditor(Type type) => Activator.CreateInstance(type) as ILocalizationActorEditor;

        //public static void CallAddComponent(Component obj)
        //{


        //    Type type = obj.GetType();
        //    if (obj is LocalizationBehavior)
        //    {
        //        var component = obj as LocalizationBehavior;
        //        var actors = component.LoadActors();
        //        foreach (var actor in actors)
        //        {
        //            var actor_type = actor.GetType();
        //            if (IsSubclassOfGeneric(typeof(LocalizationMapActor<,>), actor_type))
        //            {
        //                var laguages = component.GetLocalizationTypes();

        //                if (ExistActorEditor(actor_type, out var result))
        //                {
        //                    var editor = CreateEditor(result);
        //                    var method = result.GetMethod(nameof(LocalizationMapActorEditor<GraphicColorActor, Color, LocalizationGraphic>.EnsureMap));
        //                    method.Invoke(editor, new object[] { component, actor });
        //                }

        //            }
        //        }
        //    }
        //}


        static List<ITranslator> translators = new List<ITranslator>();
        static List<Type> translator_types;

        public static List<Type> GetTranslatorTypes()
        {
            if (translator_types == null)
            {
                translator_types = GetSubTypesInAssemblies(typeof(ITranslator)).Where(x => !x.IsAbstract).ToList();
            }
            return translator_types;
        }
        static LocalizationEditorHelper()
        {
            void CallAddComponent(Component component)
            {
                if (component is LocalizationBehavior behavior)
                {
                    var actors = behavior.LoadActors();
                    if (!Application.isPlaying)

                        foreach (var actor in actors)
                        {
                            actor.OnEditorLoad();
                        }
                    EditorUtility.SetDirty(behavior);
                    SceneView.lastActiveSceneView?.Repaint();

                }
            }
            ObjectFactory.componentWasAdded += CallAddComponent;

            translators.Clear();
            foreach (var type in GetTranslatorTypes())
            {
                ITranslator translator = Activator.CreateInstance(type) as ITranslator;
                translators.Add(translator);
            }


            actorEditors = GetSubTypesInAssemblies(typeof(ILocalizationActorEditor))
               .Where(x => !x.IsAbstract && x.GetCustomAttribute<LocalizationActorEditorAttribute>() != null)
               .Select(x => new { x, target = x.BaseType.GetGenericArguments()[0] })
               .ToDictionary(x => x.target, x => x.x);
            //ObjectFactory.componentWasAdded += CallAddComponent;

            if (!Directory.Exists(ObjDir))
                Directory.CreateDirectory(ObjDir);
            Localization.SetRecorder(context);
            LocalizationBehavior.defaultContext = LocalizationSetting.defaultData;

        }



        internal static UnityEngine.ScriptableObject LoadContext(Type type, string path)
        {
            ScriptableObject _context;
            if (!System.IO.File.Exists(path))
            {
                _context = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(_context, path);
            }
            else
            {
                _context = AssetDatabase.LoadAssetAtPath(path, type) as UnityEngine.ScriptableObject;
            }
            return _context;
        }

        private static LocalizationSetting _context;
        internal static LocalizationSetting context
        {
            get
            {
                if (_context == null)
                {
                    _context = LoadContext(typeof(LocalizationSetting), "Assets/Editor/LocalizationSetting.asset") as LocalizationSetting;
                }
                return _context;
            }
        }


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


            public static Encoding GBK => Encoding.GetEncoding("GBK");

            public static Encoding GuessStreamEncoding(Stream stream)
            {
                if (!stream.CanRead)
                    return null;
                var br = new BinaryReader(stream);
                var buffer = br.ReadBytes(3);
                if (buffer[0] == 0xFE && buffer[1] == 0xFF)//FE FF 254 255  UTF-16 BE (big-endian)
                    return Encoding.BigEndianUnicode;

                if (buffer[0] == 0xFF && buffer[1] == 0xFE)//FF FE 255 254  UTF-16 LE (little-endian)
                    return Encoding.Unicode;
                if (buffer[0] == 0xEF && buffer[1] == 0xBB & buffer[2] == 0xBF)//EF BB BF 239 187 191 UTF-8 
                    return Encoding.UTF8;//with BOM

                if (IsUtf8WithoutBom(stream))
                    return Encoding.UTF8;//without BOM
                if (IsPlainASCII(stream))
                    return Encoding.ASCII; //默认返回ascii编码
                return GBK;
            }


            private static bool IsPlainASCII(Stream stream)
            {
                bool isAllASCII = true;
                long totalLength = stream.Length;
                stream.Seek(0, SeekOrigin.Begin);//重置 position 位置
                var br = new BinaryReader(stream, Encoding.Default, true);
                for (long i = 0; i < totalLength; i++)
                {
                    byte b = br.ReadByte();
                    /*
                     * 原理是
                     * 0x80     1000 0000
                     * &
                     * 0x75 (p) 0111 0101
                     * ASCII字符都比128小，与运算自然都是0
                     */
                    if ((b & 0x80) != 0)// (1000 0000): 值小于0x80的为ASCII字符    
                    {
                        isAllASCII = false;
                        break;
                    }
                }

                return isAllASCII;
            }

            private static bool IsUtf8WithoutBom(Stream stream)
            {
                stream.Seek(0, SeekOrigin.Begin);//重置 position 位置
                bool isAllASCII = true;
                long totalLength = stream.Length;
                long nBytes = 0;
                var br = new BinaryReader(stream, Encoding.Default, true);
                for (long i = 0; i < totalLength; i++)
                {
                    byte b = br.ReadByte();
                    // (1000 0000): 值小于0x80的为ASCII字符    
                    // 等同于 if(b < 0x80 )
                    if ((b & 0x80) != 0) //0x80 128
                    {
                        isAllASCII = false;
                    }
                    if (nBytes == 0)
                    {
                        if (b >= 0x80)
                        {
                            if (b >= 0xFC && b <= 0xFD) { nBytes = 6; }//此范围内为6字节UTF-8字符
                            else if (b >= 0xF8) { nBytes = 5; }// 此范围内为5字节UTF-8字符
                            else if (b >= 0xF0) { nBytes = 4; }// 此范围内为4字节UTF-8字符    
                            else if (b >= 0xE0) { nBytes = 3; }// 此范围内为3字节UTF-8字符    
                            else if (b >= 0xC0) { nBytes = 2; }// 此范围内为2字节UTF-8字符    
                            else { return false; }
                            nBytes--;
                        }
                    }
                    else
                    {
                        if ((b & 0xC0) != 0x80) { return false; }//0xc0 192  (11000000): 值介于0x80与0xC0之间的为无效UTF-8字符    
                        nBytes--;
                    }
                }

                if (nBytes > 0) return false;
                if (isAllASCII) return false;
                return true;
            }





            public static void BeginRead(string path)
            {
                lineReg = new Regex(LocalizationSetting.lineReg);
                fieldReg = new Regex(LocalizationSetting.fieldReg);
                quotesReg = new Regex(LocalizationSetting.quotesReg);
                sbr = new StringBuilder();
                index = 0;

                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var en = GuessStreamEncoding(stream);

                    stream.Seek(0, SeekOrigin.Begin);
                    using (StreamReader reader = new StreamReader(stream, en, true))
                    {
                        txt = reader.ReadToEnd();

                    }
                }

            }
            public static float progress { get; private set; }
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
                bool inQuotes = false; // 用于标记是否在引号内

                while (txt.Length > index)
                {
                    c = txt[index];
                    index++;
                    progress = index / (float)txt.Length;
                    if (c == '"') // 如果遇到引号，切换 inQuotes 状态
                    {
                        inQuotes = !inQuotes;
                    }

                    if (c == '\n' && !inQuotes) // 如果遇到换行符且不在引号内，返回当前行
                    {
                        return sbr.ToString();
                    }
                    else
                    {
                        sbr.Append(c);
                    }
                }

                return sbr.Length > 0 ? sbr.ToString() : null;
            }
            //private static string ReadLine(string txt, ref int index)
            //{
            //    StringBuilder sbr = new StringBuilder();
            //    char c;
            //    while (txt.Length > index)
            //    {
            //        c = (char)txt[index];
            //        index++;
            //        if (c == '\n' && sbr.Length > 0 /*&& sbr[sbr.Length - 1] == '\r'*/)
            //        {
            //            sbr.Remove(sbr.Length - 1, 1);
            //            return sbr.ToString();
            //        }
            //        else
            //        {
            //            sbr.Append(c);
            //        }
            //    }
            //    return sbr.Length > 0 ? sbr.ToString() : null;
            //}
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

        private class SearchTreePop : PopupWindowContent
        {

            private class Node
            {
                public List<Node> children;
                private Dictionary<string, Node> map;
                private Dictionary<int, Node> root_map = new Dictionary<int, Node>();

                public int depth = -1;
                private int start_index;
                public string name = "Root";
                public Node parent;
                public Node root;
                public int id = 0;
                public string key;
                private Node AddChild(string name, string key)
                {
                    children = children ?? new List<Node>();
                    map = map ?? new Dictionary<string, Node>();

                    if (map.TryGetValue(name, out var result))
                    {
                        return result;
                    }
                    result = new Node()
                    {
                        root = root_node,
                        name = name,
                        parent = this,
                        depth = this.depth + 1,
                        start_index = start_index + name.Length + 1,
                    };
                    children.Add(result);
                    map[name] = result;
                    root_map[result.id] = result;
                    return result;
                }
                public void ReadKey(string key)
                {
                    var index = key.IndexOf('/', start_index);

                    if (index == -1)
                    {
                        var childName = key.Substring(start_index);
                        var child = AddChild(childName, key);
                        child.key = key;
                        //child.ReadKey(key);
                    }
                    else
                    {
                        //var first = key.Substring(start_index, index);
                        var childName = key.Substring(start_index, index - start_index);
                        var child = AddChild(childName, key);
                        child.ReadKey(key);

                    }



                }

                public void FreshIndex(ref int index)
                {
                    this.id = index;
                    index++;
                    var _map = parent == null ? this.root_map : root.root_map;
                    _map[this.id] = this;
                    if (children != null)
                    {
                        children.Sort((x, y) =>
                        {
                            var _x = x.children == null ? 0 : x.children.Count;
                            var _y = y.children == null ? 0 : y.children.Count;
                            return _y.CompareTo(_x);

                        });
                        for (int i = 0; i < children.Count; i++)
                        {
                            var child = children[i];
                            child.FreshIndex(ref index);

                        }
                    }
                }

                public Node Find(int id)
                {
                    var _root = parent == null ? this : this.root;
                    if (_root.root_map.TryGetValue(id, out var find))
                        return find;
                    return null;
                }
            }

            private class SearchTree : TreeView
            {
                private SearchTreePop pop;

                public SearchTree(SearchTreePop pop) : base(new TreeViewState())
                {
                    this.pop = pop;
                    Reload();
                }

                protected override TreeViewItem BuildRoot()
                {
                    return new TreeViewItem()
                    {
                        depth = -1
                    };
                }
                protected override void RowGUI(RowGUIArgs args)
                {
                    base.RowGUI(args);
                    if (string.IsNullOrEmpty(searchString))
                    {

                        var id = args.item.id;
                        var find = _view_node.Find(id);
                        if (find.children != null)
                        {
                            var rect = args.rowRect;
                            rect.x = rect.xMax - 20;
                            rect.width = 20;
                            GUI.Label(rect, EditorGUIUtility.IconContent("tab_next"));
                        }
                    }
                }
                protected override void SingleClickedItem(int id)
                {
                    var find = _view_node.Find(id);
                    if (find.children == null)
                    {
                        select?.Invoke(Array.IndexOf(keys, find.key));
                        pop.editorWindow.Close();
                    }
                    else
                    {
                        _view_node = find;
                        Reload();
                        SetSelection(new List<int>()
                            {
                            rootItem.children.First().id
                            });
                    }
                }

                public void ClickItem(int id)
                {
                    this.SingleClickedItem(id);
                }
                private void CreateItem(Node node, TreeViewItem parent, IList<TreeViewItem> result)
                {
                    TreeViewItem item = new TreeViewItem()
                    {
                        //depth = node.depth,
                        displayName = node.name,
                        parent = parent,
                        id = node.id,

                    };
                    result.Add(item);
                }
                private void CreateForSearch(Node node, TreeViewItem parent, IList<TreeViewItem> result)
                {
                    if (node.children == null)
                    {
                        if (node.key.Contains(this.searchString))
                        {

                            TreeViewItem item = new TreeViewItem()
                            {
                                //depth = node.depth,
                                displayName = node.key,
                                parent = parent,
                                id = node.id,

                            };
                            result.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in node.children)
                        {
                            CreateForSearch(item, parent, result);
                        }
                    }
                }
                protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
                {
                    var rows = this.GetRows() ?? new List<TreeViewItem>();
                    rows.Clear();
                    _view_node = _view_node ?? root_node;

                    if (string.IsNullOrEmpty(this.searchString))
                    {
                        if (_view_node.children != null)
                            for (int i = 0; i < _view_node.children.Count; i++)
                            {
                                var node = _view_node.children[i];
                                CreateItem(node, root, rows);
                            }
                    }
                    else
                    {
                        _view_node = root_node;
                        if (_view_node.children != null)
                            for (int i = 0; i < _view_node.children.Count; i++)
                            {
                                var node = _view_node.children[i];
                                CreateForSearch(node, root, rows);
                            }

                    }



                    SetupParentsAndChildrenFromDepths(root, rows);
                    return rows;
                }
                protected override bool CanMultiSelect(TreeViewItem item)
                {
                    return false;
                }
                public TreeViewItem root => rootItem;

            }
            public override Vector2 GetWindowSize() => size;
            private Vector2 size;

            public SearchTreePop(Vector2 size) => this.size = size;

            private SearchTree tree;
            private static Node root_node;
            private static string[] keys;
            private static Node _view_node;
            private static Action<int> select;
            public static EditorWindow Show(Rect rect, string[] keys, Action<int> call, float height)
            {
                SearchTreePop.keys = keys;
                _view_node = null;
                root_node = new Node();
                select = call;
                foreach (string key in keys)
                {
                    root_node.ReadKey(key);
                }
                int start = 100;
                root_node.FreshIndex(ref start);
                var pop = new SearchTreePop(new Vector2(rect.width, height));
                PopupWindow.Show(rect, pop);
                //pop.editorWindow?.Focus();
                return pop.editorWindow;
            }
            SearchField search = new SearchField();
            public override void OnGUI(Rect rect)
            {
                tree = tree ?? new SearchTree(this);
                Rect top = new Rect(rect.position, new Vector2(rect.width, 30));
                var value = 6;
                top.x += value;
                top.y += value;
                top.width -= value * 2;
                top.height -= value * 2;
                var temp = tree.searchString;
                tree.searchString = search.OnGUI(top, tree.searchString);
                top.x = 0; ;
                //top.y -= value;
                top.width = rect.width;
                top.height = 20;
                top.y += 20;
                top.height = 25;
                if (GUI.Button(top, _view_node.name))
                {
                    if (_view_node != root_node)
                    {
                        _view_node = _view_node.parent;
                        tree.Reload();
                    }

                }
                var _rect = new Rect(top.position, new Vector2(20, top.height));
                //_rect.y += 4;
                if (_view_node != root_node)
                    GUI.Label(_rect, EditorGUIUtility.FindTexture("tab_prev"));



                if (tree.searchString != temp)
                {
                    tree.Reload();
                }
                tree.OnGUI(new Rect(new Vector2(rect.x, top.yMax + 2), new Vector2(rect.width, rect.height - top.yMax)));

                var eve = Event.current;
                var key = eve.keyCode;
                var select = tree.GetSelection();
                if (tree.root.children.Count == 0) return;
                if (key == KeyCode.LeftArrow && eve.type == EventType.KeyUp)
                {
                    if (_view_node != root_node)
                    {
                        _view_node = _view_node.parent;
                        tree.Reload();
                        tree.SetSelection(new List<int>()
                            {
                               tree.root.children.First().id
                            });
                        tree.SetFocus();

                    }
                    Event.current.Use();
                }
                else if ((key == KeyCode.RightArrow || key == KeyCode.KeypadEnter || key == KeyCode.Return) && eve.type == EventType.KeyUp)
                {
                    if (select.Count != 0)
                    {

                        var find = _view_node.Find(select.First());
                        tree.ClickItem(find.id);
                    }
                    Event.current.Use();

                }
                else if (key == KeyCode.UpArrow)
                    tree.SetFocus();
                else if (key == KeyCode.DownArrow)
                    tree.SetFocus();


                editorWindow.Repaint();

            }
        }

        static bool win_close = false;
        static int _index;
        private static int s_CurrentControl;

        public static int AdvancedPopup(Rect rect, int selectedIndex, string[] displayedOptions, float minHeight, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID("AdvancedPopup".GetHashCode(), FocusType.Keyboard, rect);
            if (EditorGUI.DropdownButton(rect, new GUIContent(displayedOptions[selectedIndex]), FocusType.Passive))
            {
                win_close = false;
                s_CurrentControl = controlID;

                var win = SearchTreePop.Show(rect, displayedOptions, (value) =>
                {
                    _index = value;
                    win_close = true;
                }, minHeight);

            }
            if (win_close && s_CurrentControl == controlID)
            {
                s_CurrentControl = 0;
                return _index;
            }
            return selectedIndex;

        }









        public static V DrawObject<V>(string label, V value)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(V)))
            {
                UnityEngine.Object src = null;
                UnityEngine.Object result = null;

                if (value != null)
                    src = (UnityEngine.Object)(object)value;
                result = EditorGUILayout.ObjectField(label, src, typeof(V), false);
                if (result != null)
                    if (src != null)
                        if (src == result)
                            return value;
                        else
                            return (V)(object)result;
                    else
                        return (V)(object)result;
                else
                {
                    if (src != null)
                        return default;
                    else
                        return value;
                }

            }
            if (typeof(V).IsEnum)
            {

                if (typeof(V).IsDefined(typeof(System.FlagsAttribute), false))
                    return (V)(object)EditorGUILayout.EnumFlagsField(label, (Enum)(object)value);
                else
                    return (V)(object)EditorGUILayout.EnumPopup(label, (Enum)(object)value);
            }


            else if (typeof(V) == typeof(string))
            {
                string src = string.Empty;
                if (value != null)
                    src = (string)(object)value;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, GUILayout.Width(100));
                string tmp = EditorGUILayout.TextArea(src, GUILayout.MinHeight(40));
                GUILayout.EndHorizontal();
                return (V)(object)tmp;
            }
            else if (typeof(V) == typeof(int))
                return (V)(object)EditorGUILayout.IntField(label, (int)(object)value);
            else if (typeof(V) == typeof(float))
                return (V)(object)EditorGUILayout.FloatField(label, (float)(object)value);
            else if (typeof(V) == typeof(bool))
                return (V)(object)EditorGUILayout.Toggle(label, (bool)(object)value);
            else if (typeof(V) == typeof(double))
                return (V)(object)EditorGUILayout.DoubleField(label, (double)(object)value);
            else if (typeof(V) == typeof(long))
                return (V)(object)EditorGUILayout.LongField(label, (long)(object)value);
            else if (typeof(V) == typeof(Color))
                return (V)(object)EditorGUILayout.ColorField(label, (Color)(object)value);
            else if (typeof(V) == typeof(Vector3))
                return (V)(object)EditorGUILayout.Vector3Field(label, (Vector3)(object)value);
            else if (typeof(V) == typeof(Vector2))
                return (V)(object)EditorGUILayout.Vector2Field(label, (Vector2)(object)value);
            else if (typeof(V) == typeof(Vector4))
                return (V)(object)EditorGUILayout.Vector4Field(label, (Vector4)(object)value);
            else if (typeof(V) == typeof(Vector2Int))
                return (V)(object)EditorGUILayout.Vector2IntField(label, (Vector2Int)(object)value);
            else if (typeof(V) == typeof(Vector3Int))
                return (V)(object)EditorGUILayout.Vector3IntField(label, (Vector3Int)(object)value);
            else if (typeof(V) == typeof(Rect))
                return (V)(object)EditorGUILayout.RectField(label, (Rect)(object)value);
            else if (typeof(V) == typeof(RectInt))
                return (V)(object)EditorGUILayout.RectIntField(label, (RectInt)(object)value);
            else if (typeof(V) == typeof(Bounds))
                return (V)(object)EditorGUILayout.BoundsField(label, (Bounds)(object)value);
            else if (typeof(V) == typeof(AnimationCurve))
            {
                AnimationCurve src = (AnimationCurve)(object)value;
                if (src == null) src = new AnimationCurve();
                return (V)(object)EditorGUILayout.CurveField(label, src);
            }
            else if (typeof(V) == typeof(Gradient))
            {
                Gradient src = (Gradient)(object)value;
                if (src == null) src = new Gradient();
                return (V)(object)EditorGUILayout.GradientField(label, src);
            }

            return default;
        }

        internal static void SaveContext(ScriptableObject context)
        {
            EditorUtility.SetDirty(context);
            AssetDatabase.SaveAssetIfDirty(context);
            Localization.ForceRefreshBehaviors();
        }

        public static void ReadCSV(string path, LocalizationData context)
        {
            CSVHelper.BeginRead(path);
            int index = 0;
            string[] lanTypes = null;
            while (true)
            {
                EditorUtility.DisplayProgressBar($"ReadCSV {path}", $"{GetProgressTxt(CSVHelper.progress)}", CSVHelper.progress);
                var fields = CSVHelper.ReadFields();
                if (fields == null) break;
                if (index == 0)
                {
                    lanTypes = fields;
                    for (int i = 0; i < lanTypes.Length; i++)
                    {
                        lanTypes[i] = lanTypes[i].Replace("\r", "").Replace("\n", "");
                    }
                }
                else
                {
                    var key = fields[0];
                    for (int j = 1; j < fields.Length; j++)
                    {
                        var value = fields[j];
                        var language = lanTypes[j];
                        context.AddPair(language, key, value);
                    }
                }

                index++;
            }
            EditorUtility.ClearProgressBar();
            SaveContext(context);
        }
        private static string GetProgressTxt(float progress) => $"{(progress * 100).ToString("0.00")} %";
        public static void ReadExcel(string path, LocalizationData context)
        {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {

                        var FieldCount = reader.FieldCount;
                        var row_count = reader.RowCount;

                        string[] lanTypes = new string[FieldCount];
                        for (int i = 0; i < row_count; i++)
                        {
                            var progress = (float)i / row_count;

                            reader.Read();
                            EditorUtility.DisplayProgressBar($"ReadExcel {path}", $"Sheet({reader.Name})\t\t{GetProgressTxt(progress)}", progress);
                            if (i == 0)
                            {
                                for (int j = 0; j < FieldCount; j++)
                                    lanTypes[j] = reader.GetString(j);
                            }
                            else
                            {
                                var key = reader.GetString(0);
                                if (!string.IsNullOrEmpty(key))
                                {
                                    for (int j = 1; j < FieldCount; j++)
                                    {

                                        var value = reader.GetValue(j);
                                        var language = lanTypes[j];
                                        if (value != null)
                                            context.AddPair(language, key, value.ToString());
                                        else
                                            context.AddPair(language, key, string.Empty);
                                    }
                                }
                            }
                        }
                    }
                    while (reader.NextResult());
                }
            }
            EditorUtility.ClearProgressBar();
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


        public static void ClearContext<T>(ActorAsset<T> context)
        {
            context.Clear();
            SaveContext(context);
        }
        public static string ToAssetsPath(string self)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            return "Assets" + Path.GetFullPath(self).Substring(assetRootPath.Length).Replace("\\", "/");
        }
        public static void ReadContext<T>(ActorAsset<T> src, ActorAsset<T> target)
        {
            if (src == null) return;
            var types = src.GetLocalizationTypes();
            var keys = src.GetLocalizationKeys();
            foreach (var key in keys)
            {
                foreach (var type in types)
                {
                    var value = src.GetLocalization(type, key);
                    target.AddPair(type, key, value);
                }
            }
            SaveContext(target);
        }



        public static void DeleteKeys<T>(ActorAsset<T> context, List<string> keys)
        {
            context.ClearKeys(keys);

            SaveContext(context);

        }
        public static void ClearLanguage<T>(ActorAsset<T> context, string language)
        {
            context.ClearLanguage(language);


            SaveContext(context);

        }
        public static void SplitLanguage<T>(ActorAsset<T> context, string language)
        {
            var languages = context.GetLocalizationTypes();
            if (!languages.Contains(language)) return;

            var path = AssetDatabase.GetAssetPath(context);
            var dir = Path.GetDirectoryName(path);


            path = Path.Combine(dir, $"{context.name}_{language}.asset");
            var split = LoadContext(context.GetType(), path) as ActorAsset<T>;
            split.Clear();
            split.Merge(context, language);
            SaveContext(split);


            ClearLanguage(context, language);
            AssetDatabase.Refresh();
        }
        public static void AddLocalizationType(LocalizationData context, string type)
        {
            context.AddLanguage(type);
            SaveContext(context);

        }
        public static void AddLocalizationPair<T>(ActorAsset<T> context, string type, string key, T val)
        {
            context.AddPair(type, key, val);
            SaveContext(context);

        }
        private static IEnumerable<Type> GetAllSubclassesOfGenericType(this Type genericBaseType)
        {
            if (!genericBaseType.IsGenericTypeDefinition) return Enumerable.Empty<Type>();
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => IsSubclassOfGeneric(genericBaseType, type));
        }
        private static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
        private static bool IsSubclassOfGeneric(Type genericBaseType, Type type) =>
            type.GetBaseTypes()
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(t => t == genericBaseType);
        public static IEnumerable<Type> GetSubTypesInAssemblies(Type self)
        {
            if (self.IsInterface)
                return AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(item => item.GetTypes())
                                .Where(item => item.GetInterfaces().Contains(self));
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(item => item.GetTypes())
                            .Where(item => item.IsSubclassOf(self));
        }

        public static ITranslator GetTranslator()
        {
            var type = LocalizationSetting.translatorType;
            var param = LocalizationSetting.translatorParam;
            foreach (var item in translators)
            {
                if (item.GetType().FullName == type)
                {
                    return item;
                }
            }
            return null;
        }

        public static bool CanTranslate()
        {
            var tanslator = GetTranslator();
            if (tanslator == null) return false;
            return tanslator.IsValid(LocalizationSetting.translatorParam);
        }


        private static async Task _Translate(LocalizationData context, List<string> keys, string src, string dest)
        {
            List<string> _from = keys.Select(x => context.GetLocalization(src, x)).ToList();
            var tanslator = GetTranslator();
            if (tanslator == null) return;


            var result = await tanslator.Translate(LocalizationSetting.translatorParam, _from.ToArray(), src, dest);
            if (result.errorCode == 0)
                for (var i = 0; i < result.translateResults.Count; i++)
                {
                    var item = result.translateResults[i];
                    var _key = keys[i];
                    context.AddPair(dest, _key, item.translation);
                }
            else
                Debug.LogError($"from:{src}\t to{dest} ErrCode:{result.errorCode}");
        }
        public static async Task Translate(LocalizationData context, List<string> keys, string src, string dest)
        {
            int start = 0;
            int once = 20;
            while (start < keys.Count)
            {
                var end = Mathf.Min(keys.Count, start + once);
                var progress = (float)start / keys.Count;
                EditorUtility.DisplayProgressBar($"Translate {src}->{dest}", $"{progress.ToString("0.00")}", progress);

                var _keys = new List<string>();
                for (var i = start; i < end; i++)
                {
                    _keys.Add(keys[i]);
                }
                await _Translate(context, _keys, src, dest);
                start = end;
            }
            EditorUtility.ClearProgressBar();


            EditorApplication.delayCall += () =>
            {
                SaveContext(context);
            };
        }

        [MenuItem("Tools/WooLocalization/GenKeys")]
        public static void GenKeys()
        {
            string sriptName = "LocalizationKeys";
            var path = AssetDatabase.FindAssets($"t:script")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == sriptName) ?? $"Assets/{sriptName}.cs";




            string cls = $"namespace {nameof(WooLocalization)}{{\n";
            cls += "using System.Collections.Generic;\n";
            cls += $"public class {sriptName} {{\n";
            var types = typeof(ActorAsset<>).GetAllSubclassesOfGenericType().ToList();
            List<string> languages = new List<string>();
            foreach (var type in types)
            {
                var assets = AssetDatabase.FindAssets($"t:{type.Name}").Select(x => AssetDatabase.GUIDToAssetPath(x)).Select(x => AssetDatabase.LoadAssetAtPath(x, type)).ToList();
                var _type = type.BaseType.GetGenericArguments()[0];
                var method = type.GetMethod(nameof(LocalizationData.GetLocalizationKeys));
                var method2 = type.GetMethod(nameof(LocalizationData.GetLocalizationTypes));

                if (assets.Count > 0)
                {
                    var keys = assets.SelectMany(x => method.Invoke(x, null) as List<string>).Distinct();
                    var lans = assets.SelectMany(x => method2.Invoke(x, null) as List<string>).Distinct();
                    languages.AddRange(lans);
                    if (keys.Count() > 0)
                    {
                        cls += $"public class {_type.Name} {{\n";
                        foreach (var key in keys)
                        {
                            var _key = key.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("/", "_")
                                          .Replace("-", "_").Replace("&", "").Replace("|", "");
                            cls += $"public const string {_key}=\"{key}\";\n";
                        }
                        cls += "}\n";
                    }

                }
            }

            cls += "\n}\n";

            cls += $"public class Languages {{\n";

            string fields = " static List<string> languages = new List<string>{\n";

            foreach (var language in languages.Distinct())
            {
                var src = language.Replace("\r", "\n").Replace("\n", "");
                var filed = src.Replace("-", "_");
                cls += $"public const string {filed}=\"{src}\";\n";
                fields += $"{filed},\n";
            }
            fields += "};\n";
            cls += fields;
            cls += "public static List<string> GetLanguages(){return languages;}\n";
            cls += "}\n";

            cls += "\n}";

            File.WriteAllText(path, cls);
            AssetDatabase.Refresh();
        }
    }
}
