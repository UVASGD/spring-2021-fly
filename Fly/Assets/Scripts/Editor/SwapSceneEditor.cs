using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public class SwapSceneEditor : Editor
{
    private static int index = 0;

    [MenuItem("Tools/Previous Scene %#T")]
    public static void PreviousScene()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        index--;
        if (index < 0) index += scenes.Length;
        OpenScene(scenes[index]);
    }

    [MenuItem("Tools/Next Scene %T")]
    public static void NextScene()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        index = (index + 1) % scenes.Length;
        OpenScene(scenes[index]);
    }

    private static void OpenScene(EditorBuildSettingsScene scene)
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(scene.path);
    }
}
