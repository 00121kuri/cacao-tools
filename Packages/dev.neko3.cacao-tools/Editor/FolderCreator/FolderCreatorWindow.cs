using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Dev.Neko3.CacaoTools.Editor.FolderCreator
{
    /// <summary>
    /// フォルダ作成機能のエディターウィンドウ
    /// </summary>
    public class FolderCreatorWindow : EditorWindow
    {
        private string _rootFolderPath = "Assets";
        private string _itemName = "";
        private List<string> _folderNames = new List<string>();
        private Vector2 _scrollPosition;
        private bool _showTemplates = true;
        private string _newTemplateName = "";
        private int _selectedTemplateIndex = 0;
        private string[] _templateNames = new string[0];

        [MenuItem("Tools/Cacao Tools/Folder Creator")]
        public static void ShowWindow()
        {
            var window = GetWindow<FolderCreatorWindow>("Folder Creator");
            window.minSize = new Vector2(400, 500);
        }

        private void OnEnable()
        {
            InitializeDefaultValues();
            RefreshTemplateList();
        }

        private void InitializeDefaultValues()
        {
            _rootFolderPath = "Assets";
            _itemName = "";
            _folderNames = Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderTemplateManager.GetDefaultFolderNames();
        }

        private void RefreshTemplateList()
        {
            var templateNames = Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderTemplateManager.GetTemplateNames();
            _templateNames = templateNames.ToArray();

            if (_selectedTemplateIndex >= _templateNames.Length)
            {
                _selectedTemplateIndex = 0;
            }
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Label("Folder Creator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // テンプレートセクション
            DrawTemplateSection();

            GUILayout.Space(10);
            EditorGUILayout.Separator();
            GUILayout.Space(10);

            // メイン設定セクション
            DrawMainSection();

            EditorGUILayout.EndScrollView();
        }

        private void DrawTemplateSection()
        {
            _showTemplates = EditorGUILayout.Foldout(_showTemplates, "Templates", true);

            if (_showTemplates)
            {
                EditorGUI.indentLevel++;

                // テンプレート読み込み
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Load Template:", GUILayout.Width(100));

                if (_templateNames.Length > 0)
                {
                    int newIndex = EditorGUILayout.Popup(_selectedTemplateIndex, _templateNames);
                    if (newIndex != _selectedTemplateIndex)
                    {
                        _selectedTemplateIndex = newIndex;
                    }

                    if (GUILayout.Button("Load", GUILayout.Width(60)))
                    {
                        LoadTemplate(_templateNames[_selectedTemplateIndex]);
                    }

                    if (GUILayout.Button("Delete", GUILayout.Width(60)))
                    {
                        DeleteTemplate(_templateNames[_selectedTemplateIndex]);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No templates available");
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                // テンプレート保存
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Save Template:", GUILayout.Width(100));
                _newTemplateName = EditorGUILayout.TextField(_newTemplateName);

                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_newTemplateName));
                if (GUILayout.Button("Save", GUILayout.Width(60)))
                {
                    SaveCurrentAsTemplate();
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
        }

        private void DrawMainSection()
        {
            // ルートフォルダパス
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Root Folder Path:", GUILayout.Width(120));
            _rootFolderPath = EditorGUILayout.TextField(_rootFolderPath);

            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Root Folder", _rootFolderPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Unityプロジェクトパス相対に変換
                    string dataPath = Application.dataPath;
                    if (selectedPath.StartsWith(dataPath))
                    {
                        _rootFolderPath = "Assets" + selectedPath.Substring(dataPath.Length).Replace('\\', '/');
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid Path",
                            "Please select a folder within the Assets directory.", "OK");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // バリデーション表示
            if (!Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderCreatorService.IsValidFolderPath(_rootFolderPath))
            {
                EditorGUILayout.HelpBox("Invalid root folder path. Path must start with 'Assets' and exist.", MessageType.Error);
            }

            GUILayout.Space(5);

            // アイテム名
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Item Name:", GUILayout.Width(120));
            _itemName = EditorGUILayout.TextField(_itemName);
            EditorGUILayout.EndHorizontal();

            // アイテム名バリデーション
            if (!Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderCreatorService.IsValidItemName(_itemName))
            {
                if (string.IsNullOrEmpty(_itemName))
                {
                    EditorGUILayout.HelpBox("Item name is required.", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("Invalid item name. Please avoid special characters.", MessageType.Error);
                }
            }

            GUILayout.Space(10);

            // フォルダ名リスト
            EditorGUILayout.LabelField("Folder Names:", EditorStyles.boldLabel);

            for (int i = 0; i < _folderNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _folderNames[i] = EditorGUILayout.TextField(_folderNames[i]);

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _folderNames.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Folder", GUILayout.Width(100)))
            {
                _folderNames.Add("");
            }
            EditorGUILayout.EndHorizontal();

            if (_folderNames.Count == 0)
            {
                EditorGUILayout.HelpBox("At least one folder name is required.", MessageType.Warning);
            }

            GUILayout.Space(20);

            // 作成ボタン
            EditorGUI.BeginDisabledGroup(!CanCreateFolders());
            if (GUILayout.Button("Create Folders", GUILayout.Height(40)))
            {
                CreateFolders();
            }
            EditorGUI.EndDisabledGroup();
        }

        private bool CanCreateFolders()
        {
            return Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderCreatorService.IsValidFolderPath(_rootFolderPath) &&
                   Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderCreatorService.IsValidItemName(_itemName) &&
                   _folderNames.Count > 0 &&
                   _folderNames.Any(name => !string.IsNullOrEmpty(name));
        }

        private void CreateFolders()
        {
            // 空の要素を除外
            var validFolderNames = _folderNames.Where(name => !string.IsNullOrEmpty(name)).ToList();

            bool success = Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderCreatorService.CreateFolders(_rootFolderPath, _itemName, validFolderNames);

            if (success)
            {
                EditorUtility.DisplayDialog("Success",
                    $"Folders created successfully under {_rootFolderPath}/{_itemName}", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error",
                    "Failed to create folders. Please check the console for details.", "OK");
            }
        }

        private void SaveCurrentAsTemplate()
        {
            if (string.IsNullOrEmpty(_newTemplateName))
            {
                return;
            }

            var validFolderNames = _folderNames.Where(name => !string.IsNullOrEmpty(name)).ToList();

            bool success = Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderTemplateManager.SaveTemplate(_newTemplateName, _rootFolderPath, validFolderNames);

            if (success)
            {
                _newTemplateName = "";
                RefreshTemplateList();
            }
        }

        private void LoadTemplate(string templateName)
        {
            var template = Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderTemplateManager.LoadTemplate(templateName);
            if (template != null)
            {
                _rootFolderPath = template.rootFolderPath;
                _folderNames = new List<string>(template.folderNames);

                Debug.Log($"Cacao Tools: Template '{templateName}' loaded successfully.");
            }
        }

        private void DeleteTemplate(string templateName)
        {
            bool success = Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderTemplateManager.DeleteTemplate(templateName);
            if (success)
            {
                RefreshTemplateList();
            }
        }
    }
}
