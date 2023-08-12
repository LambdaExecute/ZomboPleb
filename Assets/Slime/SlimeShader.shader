Shader "Custom/SlimeShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Angle("Angle", Float) = 0
        _Frequency("Frequency", Float) = 10
        _Amplitude("Amplitude", Float) = 2
        _Speed("Speed", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _Angle;
        float _Frequency;
        float _Amplitude;
        float _Speed;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        float Wave(float2 uv, float angle, float frequency, float amplitude, float speed)
        {
            float radian = angle/180*3.14f;
            return (1 + sin((sin(radian)*uv.x + cos(radian)*uv.y)*frequency + _Time.x * speed)) * amplitude;
        }

        float FullWaves(float2 uv, int iterations)
        {
            float wave = 0;
            for(int i = 4; i <= iterations; i++)
            {
                wave += Wave(uv, pow(_Angle,i), i*_Frequency, _Amplitude/i, _Speed*i);
            }
            return wave;
        }

        float3 FullWavesNormal(float2 uv, int iterations)
        {
            float zero = FullWaves(uv,iterations);
            float rShift = FullWaves(uv + float2(0.001,0),iterations);
            float gShift = FullWaves(uv + float2(0,0.001),iterations);
            float r = zero - rShift;
            float g = zero - gShift;
            return normalize(float3(r,g,0.01));
        }

        void vert (inout appdata_full v) {
          v.vertex.xyz += v.normal * FullWaves(v.texcoord,10);
        }

       

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            o.Albedo = _Color;
            // Metallic and smoothness come from slider variables
            o.Normal = FullWavesNormal(IN.uv_MainTex,15);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
