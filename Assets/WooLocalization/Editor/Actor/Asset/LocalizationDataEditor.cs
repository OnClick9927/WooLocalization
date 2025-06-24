/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.MultiColumnHeaderState;

namespace WooLocalization
{


    [CustomEditor(typeof(LocalizationData))]
    class LocalizationDataEditor : Editor
    {
        private class Tree : TreeView
        {
            private SearchField search = new SearchField();
            private LocalizationData context => parent.context;
            private LocalizationDataEditor parent;

            public Tree(TreeViewState state, LocalizationDataEditor parent) : base(state)
            {
                this.parent = parent;
                this.showBorder = true;
                this.showAlternatingRowBackgrounds = true;
                Reload();
            }

            protected override TreeViewItem BuildRoot()
            {

                var lanTypes = context.GetLocalizationTypes();
                var columns = new List<Column>() {
                new Column()
                        {
                            autoResize = true,
                            headerContent = new GUIContent("Key"),
                        canSort = false,
                        allowToggleVisibility = false,
                        }
                };
                for (int i = 0; lanTypes.Count > i; i++)
                {
                    columns.Add(new Column()
                    {
                        autoResize = true,
                        canSort = false,
                        allowToggleVisibility = false,
                        minWidth = 100,

                        headerContent = new GUIContent(lanTypes[i])
                    });
                }
                this.multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(columns.ToArray()))
                {
                    canSort = false,
                };
                return new TreeViewItem() { depth = -1, id = 1 };
            }
            private List<TreeViewItem> _rows = new List<TreeViewItem>();
            private Dictionary<string, int> row_lines = new Dictionary<string, int>();

            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                row_lines.Clear();
                var keys = context.GetLocalizationKeys();

                var rows = new List<TreeViewItem>();
                rows.Clear();
                if (keys != null)
                {
                    var lanTypes = context.GetLocalizationTypes();

                    for (int i = 0; i < keys.Count; i++)
                    {
                        var key = keys[i];
                        bool build = string.IsNullOrEmpty(this.searchString) || key.ToLower().Contains(this.searchString.ToLower());
                        if (!build) continue;
                        int max = 1;
                        for (int j = 0; lanTypes.Count > j; j++)
                        {
                            var type = lanTypes[j];
                            string localizationText = context.GetLocalization(type, key);
                            max = Mathf.Max(localizationText.Replace("\r\n", "\n").Count(x => x == '\n'), max);
                        }
                        row_lines[key] = max;
                        rows.Add(new TreeViewItem()
                        {
                            id = i + 2,
                            displayName = key,
                            depth = 0,
                            parent = root,
                        }); ;
                    }
                }

                SetupDepthsFromParentsAndChildren(root);
                _rows = rows;
                return rows;
            }
            protected override float GetCustomRowHeight(int row, TreeViewItem item)
            {
                var key = item.displayName;
                var line_count = row_lines[key];
                return 22 * line_count;
            }
            protected override void RowGUI(RowGUIArgs args)
            {
                float indent = this.GetContentIndent(args.item);
                var key = args.item.displayName;
                Rect r = args.GetCellRect(0);
                r.x += indent;
                r.width -= indent;

                GUI.Label(r, key);

                var lanTypes = context.GetLocalizationTypes();
                for (int i = 0; lanTypes.Count > i; i++)
                {
                    var type = lanTypes[i];
                    string localizationText = context.GetLocalization(type, key);

                    GUI.Label(args.GetCellRect(i + 1), context.GetLocalization(type, key));
                }
            }
            public override void OnGUI(Rect rect)
            {
                var r1 = new Rect(rect.x, rect.y, rect.width, 20);
                var r2 = new Rect(rect.x, rect.y + 20, rect.width, rect.height - 20);

                var tmp = search.OnGUI(r1, this.searchString);
                if (tmp != this.searchString)
                {
                    this.searchString = tmp;
                    Reload();
                }
                base.OnGUI(r2);
            }
            protected override bool CanMultiSelect(TreeViewItem item)
            {
                return true;
            }
            public new void Reload()
            {
                base.Reload();
                this.multiColumnHeader.ResizeToFit();
            }
            protected override void ContextClicked()
            {
                GenericMenu menu = new GenericMenu();

                var lanTypes = context.GetLocalizationTypes();



                for (int i = 0; i < lanTypes.Count; i++)
                {
                    var type = lanTypes[i];
                    menu.AddItem(new GUIContent($"Delete Localization Type/{type}"), false, () =>
                    {
                        LocalizationEditorHelper.DeleteLocalizationType(context, type);
                        Reload();
                    });
                }

                for (int i = 0; lanTypes.Count > i; i++)
                {
                    for (int j = 0; lanTypes.Count > j; j++)
                    {
                        var src = lanTypes[i];
                        var dst = lanTypes[j];
                        if (src == dst) continue;

                        menu.AddItem(new GUIContent($"Translate/{src}To{dst}"), false, () =>
                        {
                            LocalizationEditorHelper.Translate(context, context.GetLocalizationKeys(), src, dst).Wait();

                        });
                    }

                }
                var select = this.GetSelection();
                if (search != null && select.Count > 0)
                {
                    var keys = select.Select(x => _rows.Find(y => y.id == x).displayName).ToList();

                    if (select.Count == 1)
                    {
                        var key = keys.First();
                        menu.AddItem(new GUIContent("CopySelect/Key"), false, () =>
                        {
                            GUIUtility.systemCopyBuffer = key;
                        });
                        for (int i = 0; lanTypes.Count > i; i++)
                        {
                            var type = lanTypes[i];
                            menu.AddItem(new GUIContent($"CopySelect/{type}"), false, () =>
                            {

                                GUIUtility.systemCopyBuffer = context.GetLocalization(type, key);
                            });


                        }
                    }

                    for (int i = 0; lanTypes.Count > i; i++)
                    {
                        for (int j = 0; lanTypes.Count > j; j++)
                        {
                            var src = lanTypes[i];
                            var dst = lanTypes[j];
                            if (src == dst) continue;

                            menu.AddItem(new GUIContent($"TranslateSelect/{src}To{dst}"), false, async () =>
                            {
                                await LocalizationEditorHelper.Translate(context, keys, src, dst);
                            });
                        }

                    }

                    menu.AddItem(new GUIContent("Delete Select"), false, () =>
                    {
                        LocalizationEditorHelper.DeleteKeys(context, keys);
                        Reload();

                    });
                }

                menu.ShowAsContext();
            }
        }
        private Tree tree;
        private TreeViewState state = new TreeViewState();
        private LocalizationData context;
        private void OnEnable()
        {
            context = target as LocalizationData;
            tree = new Tree(state, this);
        }
        private string LanType = "";
        private string Key = "";
        private string VAL = "";

        void WriteCSV()
        {
            var path = EditorUtility.SaveFilePanel("Save CSV",
                LocalizationSetting.lastCSVPath, $"{context.name}.csv",
                "csv");
            if (string.IsNullOrEmpty(path)) return;
            LocalizationSetting.lastCSVPath = path;
            LocalizationEditorHelper.WriteCSV(path, context);
        }

        void ReadCSV()
        {
            var path = EditorUtility.OpenFilePanelWithFilters("Select CSV", LocalizationSetting.lastCSVPath, new string[] { "CSV", "csv" });
            if (string.IsNullOrEmpty(path)) return;
            LocalizationSetting.lastCSVPath = Path.GetDirectoryName(path);
            LocalizationEditorHelper.ReadCSV(path, context);
        }




        public override void OnInspectorGUI()
        {
            if (Event.current?.commandName == "ObjectSelectorClosed")
            {
                var selectObj = EditorGUIUtility.GetObjectPickerObject();
                var other = EditorGUIUtility.GetObjectPickerObject() as LocalizationData;
                if (other != null && other != context)
                {
                    LocalizationEditorHelper.ReadContext(other, context);
                    tree.Reload();
                }
            }
            if (GUILayout.Button("Clear Data"))
            {
                LocalizationEditorHelper.ClearContext(context);
                tree.Reload();
            }
            if (GUILayout.Button("Read From Asset"))
            {
                var control = EditorGUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<LocalizationData>(null, false, "", control);
                GUIUtility.ExitGUI();

            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Read From CSV"))
            {
                ReadCSV();
                tree.Reload();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("Write To CSV"))
            {
                WriteCSV();
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();


            GUILayout.Space(10);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal();
            LanType = EditorGUILayout.TextField("Language", LanType);
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                LocalizationEditorHelper.AddLocalizationType(context, LanType);
                tree.Reload();
            }
            GUILayout.EndHorizontal();

            Key = EditorGUILayout.TextField("Key", Key);
            GUILayout.BeginHorizontal();
            VAL = EditorGUILayout.TextField("VAL", VAL);
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                LocalizationEditorHelper.AddLocalizationPair(context, LanType, Key, VAL);
                tree.Reload();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            EditorGUIUtility.GetMainWindowPosition();
            tree.OnGUI(EditorGUILayout.GetControlRect(GUILayout.MinHeight(EditorGUIUtility.GetMainWindowPosition().height - 320)));
        }
    }
}
