using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public class SwapSceneEditor : Editor
{
    public static int index;
    public static int lastIndex;

    [MenuItem("Tools/Swap Scenes %T")]
    public static void SwapScenes()
    {
        if (index != 0)
        {
            lastIndex = index;
            index = 0;
        }
        else
        {
            index = lastIndex;
        }

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(EditorSceneManager.GetSceneByBuildIndex(index).path);
    }
}
