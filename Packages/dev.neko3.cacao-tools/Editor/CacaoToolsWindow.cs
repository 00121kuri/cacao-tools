using UnityEngine;
using UnityEditor;
using System;

namespace Dev.Neko3.CacaoTools.Editor
{
    /// <summary>
    /// Cacao Tools用のエディターウィンドウ
    /// </summary>
    public class CacaoToolsWindow : EditorWindow
    {
        [MenuItem("Tools/Cacao Tools/Cacao Tools Window")]
        public static void ShowWindow()
        {
            GetWindow<CacaoToolsWindow>("Cacao Tools");
        }
        private void OnGUI()
        {
            GUILayout.Label("Cacao Tools", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Test Log"))
            {
                Debug.Log("Cacao Tools is working!");
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Open Folder Creator"))
            {
                Dev.Neko3.CacaoTools.Editor.FolderCreator.FolderCreatorWindow.ShowWindow();
            }

            GUILayout.Space(10);
            GUILayout.Label("このパッケージは正常に動作しています。", EditorStyles.helpBox);
        }
    }
}
