using UnityEngine;
using System;
using System.Collections.Generic;

namespace Dev.Neko3.CacaoTools.Editor.FolderCreator
{
    /// <summary>
    /// フォルダテンプレートの設定データ
    /// </summary>
    [CreateAssetMenu(fileName = "FolderTemplateSettings", menuName = "Cacao Tools/Folder Template Settings")]
    public class FolderTemplateSettings : ScriptableObject
    {
        [System.Serializable]
        public class Template
        {
            public string name;
            public string rootFolderPath;
            public List<string> folderNames;

            public Template()
            {
                name = "";
                rootFolderPath = "Assets";
                folderNames = new List<string> { "Animations", "Prefabs", "Materials", "Models", "Textures" };
            }

            public Template(string templateName, string rootPath, List<string> folders)
            {
                name = templateName;
                rootFolderPath = rootPath;
                folderNames = new List<string>(folders);
            }
        }

        [SerializeField]
        private List<Template> templates = new List<Template>();

        public List<Template> Templates => templates;

        /// <summary>
        /// テンプレートを追加
        /// </summary>
        public void AddTemplate(Template template)
        {
            if (template != null && !string.IsNullOrEmpty(template.name))
            {
                templates.Add(template);
            }
        }

        /// <summary>
        /// テンプレートを削除
        /// </summary>
        public bool RemoveTemplate(string templateName)
        {
            for (int i = templates.Count - 1; i >= 0; i--)
            {
                if (templates[i].name == templateName)
                {
                    templates.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// テンプレートを取得
        /// </summary>
        public Template GetTemplate(string templateName)
        {
            return templates.Find(t => t.name == templateName);
        }

        /// <summary>
        /// テンプレート名のリストを取得
        /// </summary>
        public List<string> GetTemplateNames()
        {
            List<string> names = new List<string>();
            foreach (var template in templates)
            {
                names.Add(template.name);
            }
            return names;
        }
    }
}
