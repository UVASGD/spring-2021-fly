using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration
{
    [CreateAssetMenu(fileName = "New Terrain Data", menuName = "ScriptableObjects/Terrain Data")]
    [System.Serializable]
    public class TerrainData : UpdatableData
    {
        public bool useFlatShading;
        public float uniformScale = 1f;

        [Header("Height")]
        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;

        [Header("Falloff")]
        public bool useFalloff;
        public AnimationCurve falloffCurve;
        public float falloffRadius;
        public Noise.FalloffMode falloffMode;
        public Vector2 falloffOffset;

        public float MinHeight
        {
            get
            {
                return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
            }
        }

        public float MaxHeight
        {
            get
            {
                return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            uniformScale = Mathf.Max(0.01f, uniformScale);
            falloffRadius = Mathf.Max(0f, falloffRadius);
        }
    }
}