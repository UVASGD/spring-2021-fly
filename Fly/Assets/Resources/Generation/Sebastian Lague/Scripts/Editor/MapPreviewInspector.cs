using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TerrainGeneration
{
    [CustomEditor(typeof(MapPreview))]
    public class MapPreviewInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            MapPreview preview = (MapPreview)target;

            if (DrawDefaultInspector())
            {
                preview.meshRenderer.gameObject.SetActive(preview.showPreview);

                if (preview.autoUpdate)
                {
                    preview.DrawMapInEditor();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                preview.DrawMapInEditor();
            }
        }
    }
}