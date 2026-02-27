using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class EditorSceneNavigation
{
    private const string MenuScenePath = "Assets/Scenes/Menu.unity";
    private const string GameScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("Scenes/Open Menu Scene %#1")] // Ctrl/Cmd + Shift + 1
    public static void OpenMenuScene()
    {
        OpenScene(MenuScenePath);
    }

    [MenuItem("Scenes/Open Game Scene %#2")] // Ctrl/Cmd + Shift + 2
    public static void OpenGameScene()
    {
        OpenScene(GameScenePath);
    }

    private static void OpenScene(string scenePath)
    {
        if (!System.IO.File.Exists(scenePath))
        {
            Debug.LogError($"Scene not found at path: {scenePath}");
            return;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}