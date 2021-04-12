Shader "Custom/Goal"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Opacity("Opacity", Range(0,1)) = 0.2
        _HologramTex1("Hologram 1 (RGB)", 2D) = "white" {}
        _HologramTex2("Hologram 2 (RGB)", 2D) = "white" {}
        _HologramFreq1("Hologram 1 Frequency", Float) = 1.0
        _HologramFreq2("Hologram 2 Frequency", Float) = 1.0
        _HologramScale1("Hologram 1 Scale", Float) = 1.0
        _HologramScale2("Hologram 2 Scale", Float) = 1.0
    }
    SubShader
    {
        Tags {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        half _Opacity;
        fixed4 _Color;

        sampler2D _HologramTex1;
        sampler2D _HologramTex2;
        half _HologramFreq1;
        half _HologramFreq2;
        half _HologramScale1;
        half _HologramScale2;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        
            float rand(float2 c) {
            float num = sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453;
            return num - floor(num);
        }

        float noise(float2 p, float freq) {
            float unit = 1 / freq;
            float2 ij = floor(p / unit);
            float2 xy = fmod(p, unit) / unit;
            //xy = 3.*xy*xy-2.*xy*xy*xy;
            xy = .5 * (1. - cos(3.14159 * xy));
            float a = rand((ij + float2(0., 0.)));
            float b = rand((ij + float2(1., 0.)));
            float c = rand((ij + float2(0., 1.)));
            float d = rand((ij + float2(1., 1.)));
            float x1 = lerp(a, b, xy.x);
            float x2 = lerp(c, d, xy.x);
            return lerp(x1, x2, xy.y);
        }

        float pNoise(float2 p, int res) {
            float persistance = .5;
            float n = 0.;
            float normK = 0.;
            float f = 4.;
            float amp = 1.;
            int iCount = 0;
            for (int i = 0; i < 50; i++) {
                n += amp * noise(p, f);
                f *= 2.;
                normK += amp;
                amp *= persistance;
                if (iCount == res) break;
                iCount++;
            }
            float nf = n / normK;
            return nf * nf * nf * nf;
        }


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            float t1 = _Time * 2 * 3.14159 * _HologramFreq1;
            float t2 = _Time * 2 * 3.14159 * _HologramFreq2;
            float2 uv1 = float2(IN.uv_MainTex.x, IN.uv_MainTex.y * _HologramScale1 - t1);
            float2 uv2 = float2(IN.uv_MainTex.x, IN.uv_MainTex.y * _HologramScale2 - t2);

            fixed4 h1 = tex2D(_HologramTex1, uv1);
            fixed4 h2 = tex2D(_HologramTex2, uv2);
            
            float n = pNoise(uv2, 8);

            o.Albedo = c;
            o.Emission = h1 / 2;
            o.Alpha = _Opacity;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
