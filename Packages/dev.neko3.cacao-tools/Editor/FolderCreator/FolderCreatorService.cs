using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Dev.Neko3.CacaoTools.Editor.FolderCreator
{
    /// <summary>
    /// フォルダ作成を処理するサービスクラス
    /// </summary>
    public static class FolderCreatorService
    {
        /// <summary>
        /// 指定されたルートフォルダ配下にアイテムフォルダとその中にテンプレートフォルダを作成
        /// </summary>
        /// <param name="rootFolderPath">ルートフォルダパス</param>
        /// <param name="itemName">アイテム名</param>
        /// <param name="folderNames">作成するフォルダ名のリスト</param>
        /// <returns>作成に成功した場合true</returns>
        public static bool CreateFolders(string rootFolderPath, string itemName, List<string> folderNames)
        {
            try
            {
                // バリデーション
                if (string.IsNullOrEmpty(rootFolderPath))
                {
                    Debug.LogError("Cacao Tools: Root folder path is required.");
                    return false;
                }

                if (string.IsNullOrEmpty(itemName))
                {
                    Debug.LogError("Cacao Tools: Item name is required.");
                    return false;
                }

                if (folderNames == null || folderNames.Count == 0)
                {
                    Debug.LogError("Cacao Tools: At least one folder name is required.");
                    return false;
                }

                // ルートフォルダの存在確認
                if (!AssetDatabase.IsValidFolder(rootFolderPath))
                {
                    Debug.LogError($"Cacao Tools: Root folder does not exist: {rootFolderPath}");
                    return false;
                }

                // アイテムフォルダのパス
                string itemFolderPath = Path.Combine(rootFolderPath, itemName).Replace('\\', '/');

                // アイテムフォルダが既に存在するかチェック
                if (AssetDatabase.IsValidFolder(itemFolderPath))
                {
                    Debug.LogWarning($"Cacao Tools: Item folder already exists: {itemFolderPath}");
                }
                else
                {
                    // アイテムフォルダを作成
                    string guid = AssetDatabase.CreateFolder(rootFolderPath, itemName);
                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogError($"Cacao Tools: Failed to create item folder: {itemFolderPath}");
                        return false;
                    }
                    Debug.Log($"Cacao Tools: Created item folder: {itemFolderPath}");
                }

                // 各テンプレートフォルダを作成
                int createdCount = 0;
                foreach (string folderName in folderNames)
                {
                    if (string.IsNullOrEmpty(folderName))
                    {
                        continue;
                    }

                    string subFolderPath = Path.Combine(itemFolderPath, folderName).Replace('\\', '/');

                    if (AssetDatabase.IsValidFolder(subFolderPath))
                    {
                        Debug.LogWarning($"Cacao Tools: Folder already exists: {subFolderPath}");
                        continue;
                    }

                    string subGuid = AssetDatabase.CreateFolder(itemFolderPath, folderName);
                    if (!string.IsNullOrEmpty(subGuid))
                    {
                        createdCount++;
                        Debug.Log($"Cacao Tools: Created folder: {subFolderPath}");
                    }
                    else
                    {
                        Debug.LogError($"Cacao Tools: Failed to create folder: {subFolderPath}");
                    }
                }

                // AssetDatabaseを更新
                AssetDatabase.Refresh();

                Debug.Log($"Cacao Tools: Folder creation completed. Created {createdCount} folders under {itemFolderPath}");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Cacao Tools: Exception during folder creation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// フォルダパスの妥当性をチェック
        /// </summary>
        /// <param name="folderPath">チェックするフォルダパス</param>
        /// <returns>有効な場合true</returns>
        public static bool IsValidFolderPath(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return false;
            }

            // Assetsフォルダ配下でなければならない
            if (!folderPath.StartsWith("Assets"))
            {
                return false;
            }

            return AssetDatabase.IsValidFolder(folderPath);
        }

        /// <summary>
        /// アイテム名の妥当性をチェック
        /// </summary>
        /// <param name="itemName">チェックするアイテム名</param>
        /// <returns>有効な場合true</returns>
        public static bool IsValidItemName(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                return false;
            }

            // 無効な文字をチェック
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                if (itemName.Contains(c.ToString()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
