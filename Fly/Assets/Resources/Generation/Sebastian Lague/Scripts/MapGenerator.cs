using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace TerrainGeneration
{
    public class MapGenerator : MonoBehaviour
    {
        #region VARIABLES
        public const int mapChunkSize = 95;

        public NoiseData noiseData;
        public TerrainData terrainData;
        public TextureData textureData;

        public Material terrainMaterial;


        private Queue<ThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<ThreadInfo<MapData>>();
        private Queue<ThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ThreadInfo<MeshData>>();
        #endregion

        #region THREADING
        public void RequestMapData(Vector2 center, Action<MapData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MapDataThread(center, callback);
            };

            new Thread(threadStart).Start();
        }

        private void MapDataThread(Vector2 center, Action<MapData> callback)
        {
            MapData mapData = GenerateMapData(center);
            lock (mapDataThreadInfoQueue)
            {
                mapDataThreadInfoQueue.Enqueue(new ThreadInfo<MapData>(callback, mapData));
            }
        }

        public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MeshDataThread(mapData, lod, callback);
            };

            new Thread(threadStart).Start();
        }

        private void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, terrainData.useFlatShading);
            lock (meshDataThreadInfoQueue)
            {
                meshDataThreadInfoQueue.Enqueue(new ThreadInfo<MeshData>(callback, meshData));
            }
        }
        #endregion

        

        public MapData GenerateMapData(Vector2 center)
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.noiseScale, 
                noiseData.octaves, noiseData.persistance, noiseData.lacunarity, 
                center + noiseData.noiseOffset, noiseData.normalizeMode, noiseData.seed);

            if (terrainData.useFalloff)
            {
                float[,] falloffMap = Noise.GenerateFalloffMap(terrainData.falloffCurve, terrainData.falloffMode, mapChunkSize + 2, terrainData.falloffRadius, center);
                Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
                for (int y = 0; y < mapChunkSize + 2; y++)
                {
                    for (int x = 0; x < mapChunkSize + 2; x++)
                    {
                        if (terrainData.useFalloff)
                        {
                            noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                        }
                    }
                }
            }

            textureData.UpdateMeshHeights(terrainMaterial, terrainData.MinHeight, terrainData.MaxHeight);

            return new MapData(noiseMap);
        }

        #region MESSAGES
        private void Update()
        {
            while (mapDataThreadInfoQueue.TryDequeue(out var threadInfo))
            {
                threadInfo.callback(threadInfo.parameter);
            }

            while (meshDataThreadInfoQueue.TryDequeue(out var threadInfo))
            {
                threadInfo.callback(threadInfo.parameter);
            }
        }
        #endregion
    }
}