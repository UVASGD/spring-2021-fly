using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TerrainGeneration
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            MapGenerator generator = (MapGenerator)target;

            if (DrawDefaultInspector())
            {
                MapPreview preview = generator.GetComponent<MapPreview>();
                if (preview && preview.autoUpdate)
                {
                    preview.DrawMapInEditor();
                }
            }
        }
    }
}