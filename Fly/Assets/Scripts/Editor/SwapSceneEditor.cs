using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SwapSceneEditor : Editor
{
    public static int index;

    [MenuItem("Tools/Swap Scenes %T")]
    public static void SwapScenes()
    {
        if (index != 0) index = 0;
        else index = 1;
        string scenePath = index == 0 ? "Assets/Scenes/Menu.unity" : "Assets/Scenes/SampleScene.unity";
        UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
    }
}
