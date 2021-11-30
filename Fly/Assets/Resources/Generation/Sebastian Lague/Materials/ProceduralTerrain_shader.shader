Shader "Custom/ProceduralTerrain"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"{}
        _Scale("Scale", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        float _Scale;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
        };

        const static int maxLayerCount = 8;
        const static float epsilon = 1E-4;

        int layerCount;
        float3 baseColors[maxLayerCount];
        float baseStartHeights[maxLayerCount];
        float baseBlends[maxLayerCount];
        float baseColorStrengths[maxLayerCount];
        float baseTextureScales[maxLayerCount];

        UNITY_DECLARE_TEX2DARRAY(baseTextures);

        float minHeight;
        float maxHeight;

        float inverseLerp(float a, float b, float value) 
        {
            return saturate((value - a)/(b-a));
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float height = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
            o.Albedo = height;

            for (int i = 0; i < layerCount; i++)
            {
                float halfBlend = baseBlends[i] / 2;
                float drawStrength = inverseLerp(-halfBlend - epsilon, halfBlend, height - baseStartHeights[i]);
                o.Albedo = o.Albedo * (1 - drawStrength) + baseColors[i] * drawStrength;
            }

            float3 scaledWorldPos = IN.worldPos / _Scale;
            float3 blendAxes = abs(IN.worldNormal);
            blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
            float3 xProj = tex2D(_MainTex, scaledWorldPos.yz) * blendAxes.x;
            float3 yProj = tex2D(_MainTex, scaledWorldPos.xz) * blendAxes.y;
            float3 zProj = tex2D(_MainTex, scaledWorldPos.xy) * blendAxes.z;
 
        }
        ENDCG
    }
    FallBack "Diffuse"
}
