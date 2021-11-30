using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration
{
    [CreateAssetMenu(fileName = "New Noise Data", menuName = "ScriptableObjects/Noise Data")]
    [System.Serializable]
    public class NoiseData : UpdatableData
    {
        public int seed;
        public float noiseScale;
        public Vector2 noiseOffset;
        [Range(0, 16)]
        public int octaves;
        [Range(0f, 1f)]
        public float persistance;
        public float lacunarity;
        public Noise.NormalizeMode normalizeMode;

        protected override void OnValidate()
        {
            base.OnValidate();

            noiseScale = Mathf.Max(0.001f, noiseScale);
            lacunarity = Mathf.Max(1f, lacunarity);
        }
    }
}