using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Dev.Neko3.CacaoTools.Editor.FolderCreator
{
    /// <summary>
    /// フォルダテンプレートを管理するマネージャークラス
    /// </summary>
    public static class FolderTemplateManager
    {
        private const string SETTINGS_PATH = "Assets/CacaoTools/Settings/FolderTemplateSettings.asset";
        private const string SETTINGS_FOLDER = "Assets/CacaoTools/Settings";

        private static FolderTemplateSettings _settings;

        /// <summary>
        /// 設定を取得（なければ作成）
        /// </summary>
        public static FolderTemplateSettings GetSettings()
        {
            if (_settings == null)
            {
                _settings = AssetDatabase.LoadAssetAtPath<FolderTemplateSettings>(SETTINGS_PATH);

                if (_settings == null)
                {
                    _settings = CreateSettings();
                }
            }

            return _settings;
        }

        /// <summary>
        /// 設定ファイルを作成
        /// </summary>
        private static FolderTemplateSettings CreateSettings()
        {
            // フォルダが存在しない場合は作成
            if (!AssetDatabase.IsValidFolder(SETTINGS_FOLDER))
            {
                string[] folders = SETTINGS_FOLDER.Split('/');
                string currentPath = folders[0]; // "Assets"

                for (int i = 1; i < folders.Length; i++)
                {
                    string nextPath = currentPath + "/" + folders[i];
                    if (!AssetDatabase.IsValidFolder(nextPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, folders[i]);
                    }
                    currentPath = nextPath;
                }
            }

            // 設定ファイルを作成
            var settings = ScriptableObject.CreateInstance<FolderTemplateSettings>();
            AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Cacao Tools: Created template settings at {SETTINGS_PATH}");
            return settings;
        }

        /// <summary>
        /// テンプレートを保存
        /// </summary>
        public static bool SaveTemplate(string templateName, string rootFolderPath, List<string> folderNames)
        {
            if (string.IsNullOrEmpty(templateName))
            {
                Debug.LogError("Cacao Tools: Template name is required.");
                return false;
            }

            var settings = GetSettings();

            // 既存のテンプレートをチェック
            if (settings.GetTemplate(templateName) != null)
            {
                if (!EditorUtility.DisplayDialog("Template Exists",
                    $"Template '{templateName}' already exists. Do you want to overwrite it?",
                    "Overwrite", "Cancel"))
                {
                    return false;
                }

                settings.RemoveTemplate(templateName);
            }

            // 新しいテンプレートを作成
            var template = new FolderTemplateSettings.Template(templateName, rootFolderPath, folderNames);
            settings.AddTemplate(template);

            // 保存
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();

            Debug.Log($"Cacao Tools: Template '{templateName}' saved successfully.");
            return true;
        }

        /// <summary>
        /// テンプレートを読み込み
        /// </summary>
        public static FolderTemplateSettings.Template LoadTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
            {
                return null;
            }

            var settings = GetSettings();
            return settings.GetTemplate(templateName);
        }

        /// <summary>
        /// テンプレートを削除
        /// </summary>
        public static bool DeleteTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
            {
                return false;
            }

            if (!EditorUtility.DisplayDialog("Delete Template",
                $"Are you sure you want to delete template '{templateName}'?",
                "Delete", "Cancel"))
            {
                return false;
            }

            var settings = GetSettings();
            bool result = settings.RemoveTemplate(templateName);

            if (result)
            {
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                Debug.Log($"Cacao Tools: Template '{templateName}' deleted successfully.");
            }

            return result;
        }

        /// <summary>
        /// 全テンプレート名を取得
        /// </summary>
        public static List<string> GetTemplateNames()
        {
            var settings = GetSettings();
            return settings.GetTemplateNames();
        }

        /// <summary>
        /// デフォルトのフォルダ名リストを取得
        /// </summary>
        public static List<string> GetDefaultFolderNames()
        {
            return new List<string> { "Animations", "Prefabs", "Materials", "Models", "Textures" };
        }
    }
}
