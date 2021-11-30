using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode, int seed)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];
            
            if (seed < 0)
            {
                seed = Random.Range(0, int.MaxValue);
            }

            float maxGlobalHeight = 0;
            float amplitude = 1f;
            float frequency = 1f;

            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) - offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxGlobalHeight += amplitude;
                amplitude *= persistance;
            }

            float maxLocalHeight = float.MinValue;
            float minLocalHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    amplitude = 1f;
                    frequency = 1f;
                    float noiseHeight = 0f;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                        float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;

                    }
                    maxLocalHeight = Mathf.Max(maxLocalHeight, noiseHeight);
                    minLocalHeight = Mathf.Min(minLocalHeight, noiseHeight);
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (normalizeMode == NormalizeMode.Local)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalHeight, maxLocalHeight, noiseMap[x, y]);
                    } 
                    else
                    {
                        float normalizedHeight = (noiseMap[x, y] + maxGlobalHeight) / (2 * maxGlobalHeight);
                        noiseMap[x, y] = Mathf.Clamp01(normalizedHeight * 1.15f);
                    }
                }
            }
            return noiseMap;
        }

        public static float[,] GenerateFalloffMap(AnimationCurve function, FalloffMode falloffMode, int size, float distanceMultiplier, Vector2 offset)
        {
            float[,] map = new float[size, size];

            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < size; i++)
                {
                    float x = ((i / (float)size + offset.x) * 2 - 1) / distanceMultiplier;
                    float y = ((j / (float)size - offset.y) * 2 - 1) / distanceMultiplier;

                    float value = falloffMode == FalloffMode.Radial ? Mathf.Sqrt(x * x + y * y) : Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                    map[i, j] = function.Evaluate(value);
                }
            }

            return map;
        }

        public enum NormalizeMode
        {
            Local,
            Global
        }

        public enum FalloffMode
        {
            Radial,
            Square
        }
    }
}