using UnityEngine;
using System;

namespace TerrainGeneration
{
    [Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }

    [Serializable]
    public struct MapData
    {
        public readonly float[,] heightMap;

        public MapData(float[,] heightMap)
        {
            this.heightMap = heightMap;
        }
    }

    [Serializable]
    public struct ThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public ThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

    [Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDistanceThreshold;
    }
}