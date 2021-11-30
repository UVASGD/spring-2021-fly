using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration
{
    public class MapPreview : MonoBehaviour
    {
        public enum DrawMode
        {
            Noise,
            Mesh,
            Falloff,
        }

        public bool showPreview;
        public bool autoUpdate;
        public DrawMode drawMode;
        [Range(0, 6)]
        public int previewLOD;

        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public MapGenerator mapGenerator;

        private void Start()
        {
            meshRenderer.gameObject.SetActive(false);
        }

        private void OnValuesUpdated()
        {
            if (!Application.isPlaying)
            {
                DrawMapInEditor();
            }
        }

        private void OnTextureValuesUpdated()
        {
            mapGenerator.textureData.ApplyToMaterial(mapGenerator.terrainMaterial);
        }

        private void OnValidate()
        {
            if (mapGenerator.terrainData)
            {
                mapGenerator.terrainData.OnValuesUpdated -= OnValuesUpdated;
                mapGenerator.terrainData.OnValuesUpdated += OnValuesUpdated;
            }
            if (mapGenerator.noiseData)
            {
                mapGenerator.noiseData.OnValuesUpdated -= OnValuesUpdated;
                mapGenerator.noiseData.OnValuesUpdated += OnValuesUpdated;
            }
            if (mapGenerator.textureData)
            {
                mapGenerator.textureData.OnValuesUpdated -= OnTextureValuesUpdated;
                mapGenerator.textureData.OnValuesUpdated += OnTextureValuesUpdated;
            }
        }

        public void DrawMapInEditor()
        {
            MapData mapData = mapGenerator.GenerateMapData(Vector2.zero);

            if (drawMode == DrawMode.Noise)
            {
                DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, mapGenerator.terrainData.meshHeightMultiplier, mapGenerator.terrainData.meshHeightCurve, previewLOD, mapGenerator.terrainData.useFlatShading));
            }
            else if (drawMode == DrawMode.Falloff)
            {
                DrawTexture(TextureGenerator.TextureFromHeightMap(
                    Noise.GenerateFalloffMap(mapGenerator.terrainData.falloffCurve, mapGenerator.terrainData.falloffMode, MapGenerator.mapChunkSize + 2, mapGenerator.terrainData.falloffRadius, mapGenerator.terrainData.falloffOffset)));
            }

            meshRenderer.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;
        }

        public void DrawTexture(Texture2D texture)
        {
            float[,] meshHeightMap = new float[texture.width, texture.height];
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    meshHeightMap[x, y] = 0f;
                }
            }
            meshFilter.sharedMesh = MeshGenerator.GenerateTerrainMesh(meshHeightMap, 0f, AnimationCurve.Linear(0f, 0f, 1f, 1f), 6, mapGenerator.terrainData.useFlatShading).CreateMesh();
            meshRenderer.sharedMaterial.mainTexture = texture;
        }

        public void DrawMesh(MeshData meshData)
        {
            meshFilter.sharedMesh = meshData.CreateMesh();
            meshFilter.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;
        }

        
    }
}