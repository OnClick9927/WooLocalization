/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.MultiColumnHeaderState;


namespace WooLocalization
{

    public abstract class ActorAssetEditor<Value, Asset> : Editor where Asset : ActorAsset<Value>
    {
        private class Tree : TreeView
        {
            private SearchField search = new SearchField();
            private ActorAssetEditor<Value, Asset> parent;
            private ActorAsset<Value> context => parent.context;

            public Tree(TreeViewState state, ActorAssetEditor<Value, Asset> parent) : base(state)
            {
                this.showBorder = true;
                this.showAlternatingRowBackgrounds = true;
                this.parent = parent;
                Reload();
            }

            protected override TreeViewItem BuildRoot()
            {

                //var lanTypes = GetLocalizationTypes();
                var columns = new List<Column>() {
                new Column()
                        {
                            autoResize = true,
                            headerContent = new GUIContent("Key"),
                        canSort = false,
                        allowToggleVisibility = false,
                        }
                };
                for (int i = 0; LocalizationTypes.Count > i; i++)
                {
                    columns.Add(new Column()
                    {
                        autoResize = true,
                        canSort = false,
                        allowToggleVisibility = false,
                        minWidth = 100,

                        headerContent = new GUIContent(LocalizationTypes[i])
                    });
                }
                this.multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(columns.ToArray()))
                {
                    canSort = false,
                };
                return new TreeViewItem() { depth = -1, id = 1 };
            }

            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                var keys = context.GetLocalizationKeys();

                var rows = this.GetRows() ?? new List<TreeViewItem>();
                rows.Clear();
                if (keys != null)
                {
                    //var lanTypes = LocalizationTypes();

                    for (int i = 0; i < keys.Count; i++)
                    {
                        var key = keys[i];
                        bool build = string.IsNullOrEmpty(this.searchString) || key.ToLower().Contains(this.searchString.ToLower());
                        if (!build) continue;
                        //int max = 1;
                        //for (int j = 0; lanTypes.Count > j; j++)
                        //{
                        //    var type = lanTypes[j];
                        //    Value localizationText = context.GetValue(type, key);
                        //    //max = Mathf.Max(localizationText.Replace("\r\n", "\n").Count(x => x == '\n'), max);
                        //}
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
                return rows;
            }
            protected override float GetCustomRowHeight(int row, TreeViewItem item) => parent.GetRowHeight();
            protected override void RowGUI(RowGUIArgs args)
            {
                float indent = this.GetContentIndent(args.item);
                var key = args.item.displayName;
                Rect r = args.GetCellRect(0);
                r.x += indent;
                r.width -= indent;

                GUI.Label(r, key);

                //var lanTypes = GetLocalizationTypes();
                for (int i = 0; LocalizationTypes.Count > i; i++)
                {
                    var lan = LocalizationTypes[i];
                    Value value = context.GetLocalization(lan, key);
                    var rect = args.GetCellRect(i + 1);
                    var tmp = this.parent.DrawField(rect, value);
                    context.Add(lan, key, tmp);
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
                return false;
            }
            public new void Reload()
            {
                base.Reload();
                this.multiColumnHeader.ResizeToFit();
            }

            protected override void ContextClicked()
            {
                GenericMenu menu = new GenericMenu();

                //var lanTypes = GetLocalizationTypes();




                var select = this.GetSelection();
                if (search != null && select.Count > 0)
                {
                    var keys = GetRows().Where(y => select.Contains(y.id)).Select(x => x.displayName).ToList();

                    if (select.Count == 1)
                    {
                        var key = keys.First();
                        menu.AddItem(new GUIContent("CopySelect_Key"), false, () =>
                        {
                            GUIUtility.systemCopyBuffer = key;
                        });

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
        private ActorAsset<Value> context;
        private static List<string> LocalizationTypes;
        protected abstract Value DrawField(Rect pos, Value src);
        protected virtual float GetRowHeight() => 18;
        private void OnEnable()
        {
            LocalizationTypes = LocalizationSetting.defaultData.GetLocalizationTypes();
            context = target as ActorAsset<Value>;
            tree = new Tree(state, this);
        }
        private int lan_index;
        private string key;
        private Value value;
        private int control;
  


        public override void OnInspectorGUI()
        {

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Data"))
            {
                LocalizationEditorHelper.ClearContext(context);
                tree.Reload();
            }
            if (GUILayout.Button("Merge"))
            {
                control = EditorGUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<Asset>(null, false, "", control);
                GUIUtility.ExitGUI();
            }


            if (Event.current?.commandName == "ObjectSelectorClosed")
            {
                var selectObj = EditorGUIUtility.GetObjectPickerObject();
                var other = EditorGUIUtility.GetObjectPickerObject() as ActorAsset<Value>;
                if (other != null && other != context)
                {
                    LocalizationEditorHelper.ReadContext(other, context);
                    tree.Reload();
                }
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            lan_index = EditorGUILayout.Popup("Language", lan_index, LocalizationTypes.ToArray());
            key = EditorGUILayout.TextField(nameof(key), key);
            value = LocalizationEditorHelper.DrawObject(nameof(value), value);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(50)))
            {
                if (string.IsNullOrEmpty(key))
                {
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("key can't be null"));
                }
                LocalizationEditorHelper.AddLocalizationPair(context, LocalizationTypes[lan_index], key, value);
                tree.Reload();
            }
            GUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();


            tree.OnGUI(EditorGUILayout.GetControlRect(GUILayout.MinHeight(EditorGUIUtility.GetMainWindowPosition().height - 320)));

        }
    }
}
