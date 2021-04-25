using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneAttribute))]
public class SceneDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.propertyType == SerializedPropertyType.String)
        {
            var sceneObject = GetSceneObject(property.stringValue);
            var scene = EditorGUI.ObjectField(position, label, sceneObject, typeof(SceneAsset), false);
            if (scene == null)
            {
                property.stringValue = "";
            }
            else if (scene.name != property.stringValue)
            {
                var sceneObj = GetSceneObject(scene.name);
                if (sceneObj == null)
                {
                    Debug.LogWarning("The scene " + scene.name + " cannot be used. To use this scene add it to the build settings for the project");
                }
                else
                {
                    property.stringValue = scene.name;
                }
            }
        }
        else
            EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
    }

    protected int GetSceneBuildIndex(string sceneObjectName)
    {
        foreach (var editorScene in EditorBuildSettings.scenes)
        {
            int index = editorScene.path.IndexOf(sceneObjectName);
            if (index != -1)
            {
                return index;
            }
        }
        return -1;
    }

    protected SceneAsset GetSceneObject(string sceneObjectName)
    {
        if (string.IsNullOrEmpty(sceneObjectName))
        {
            return null;
        }

        string path = "";
        SceneAsset obj = null;

        foreach (var editorScene in EditorBuildSettings.scenes)
        {
            if (editorScene.path.IndexOf(sceneObjectName) != -1)
            {
                path = editorScene.path;
                obj = AssetDatabase.LoadAssetAtPath(editorScene.path, typeof(SceneAsset)) as SceneAsset;
                break;
            }
        }
        if (obj != null)
        {
            if (path.Contains("Scenes/Levels"))
            {
                Debug.LogWarning("Scene [" + sceneObjectName + "] cannot be used. Must be in 'Assets/Scenes/Levels' folder");
                return null;
            }
            return obj;
        }

        Debug.LogWarning("Scene [" + sceneObjectName + "] cannot be used. Add this scene to the 'Scenes in the Build' in build settings.");
        return null;
    }
}
