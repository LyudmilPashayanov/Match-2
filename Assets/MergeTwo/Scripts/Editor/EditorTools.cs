using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MergeTwo
{
    public class EditorTools
    {
        [MenuItem("Tools/MergeTwo/Open PERISITENT_DATA_PATH")]
        public static void OpenUserSaves()
        {
#if UNITY_IOS
        EditorUtility.RevealInFinder(Application.persistentDataPath);
#else
            Open(Application.persistentDataPath);
#endif
        }

        [MenuItem("Tools/MergeTwo/Delete save")]
        public static void DeleteSave()
        {
            FileManager.DeleteSave();
        }

        public static void Open(string path)
        {
            bool openInsidesOfFolder = false;
            string winPath = path.Replace("/", "\\");
            if (System.IO.Directory.Exists(winPath))
            {
                openInsidesOfFolder = true;
            }
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                e.HelpLink = "";
            }
        }

        [MenuItem("Tools/MergeTwo/Config Scene")]
        public static void OpenConfigScene()
        {
            LoadScene("Assets/Scenes/ConfigScene.unity");
        }

        [MenuItem("Tools/MergeTwo/Game Scene")]
        public static void OpenGameScene()
        {
            LoadScene("Assets/Scenes/GameScene.unity");
        }

        [MenuItem("Tools/MergeTwo/Lobby Scene")]
        public static void OpenLobbyScene()
        {
            LoadScene("Assets/Scenes/Lobby.unity");
        }


        private static void LoadScene(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.SaveOpenScenes();

            EditorSceneManager.OpenScene(path);

        }


        [MenuItem("Tools/MergeTwo/Disable auto refresh")]
        public static void DisableAutoRefresh()
        {
            EditorPrefs.SetInt("kAutoRefresh", 0);
        }
    }
}
