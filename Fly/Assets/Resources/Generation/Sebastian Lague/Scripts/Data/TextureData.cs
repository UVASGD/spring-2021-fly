using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TerrainGeneration
{
    [CreateAssetMenu(fileName = "New Texture Data", menuName = "ScriptableObjects/Texture Data")]
    [System.Serializable]
    public class TextureData : UpdatableData
    {
        const int textureSize = 512;
        const TextureFormat textureFormat = TextureFormat.RGB565;

        public Layer[] layers;
        //public Color[] baseColors;
        //[Range(0, 1)]
        //public float[] baseStartHeights;
        //[Range(0, 1)]
        //public float[] baseBlends;

        private float savedMinHeight;
        private float savedMaxHeight;
        public void ApplyToMaterial(Material material)
        {
            material.SetInt("layerCount", layers.Length);
            material.SetColorArray("baseColors", layers.Select(layer => layer.tint).ToArray());
            material.SetFloatArray("baseStartHeights", layers.Select(layer => layer.startHeight).ToArray());
            material.SetFloatArray("baseBlends", layers.Select(layer => layer.blendStrength).ToArray());
            material.SetFloatArray("baseColorStrengths", layers.Select(layer => layer.tintStrength).ToArray());
            material.SetFloatArray("baseTextureScales", layers.Select(layer => layer.textureScale).ToArray());
            material.SetTexture("baseTextures", GenerateTextureArray(layers.Select(layer => layer.texture).ToArray()));

            UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
        }

        public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
        {
            savedMinHeight = minHeight;
            savedMaxHeight = maxHeight;
            material.SetFloat("minHeight", minHeight);
            material.SetFloat("maxHeight", maxHeight);
        }

        private Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
            for (int i = 0; i < textures.Length; i++)
            {
                textureArray.SetPixels(textures[i].GetPixels(), i);
            }
            textureArray.Apply();

            return textureArray;
        }

        [System.Serializable]
        public class Layer
        {
            public Texture2D texture;
            public float textureScale = 1f;
            public Color tint;
            [Range(0, 1)]
            public float tintStrength = 1f;
            [Range(0, 1)]
            public float startHeight = 0f;
            [Range(0, 1)]
            public float blendStrength = 0.125f;
        }
    }
}