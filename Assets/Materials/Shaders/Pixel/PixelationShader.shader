Shader "Custom Post-Processing/PixelationEffect"
{
 Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _PixelSize("Pixel Size", Range(1, 100)) = 10
        _GlitchStrength("Glitch Strength", Range(0, 1)) = 0.1
        _ScanlineIntensity("Scanline Intensity", Range(0, 1)) = 0.1
        _StaticNoiseStrength("Static Noise Strength", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelSize;
            float _GlitchStrength;
            float _ScanlineIntensity;
            float _StaticNoiseStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Pixelation Effect
                float2 pixelUV = floor(i.uv * _PixelSize) / _PixelSize;

                //GLICHING
                float2 glitchOffset = float2(sin(_Time.y * 10.0), 0) * _GlitchStrength;
                float r = tex2D(_MainTex, pixelUV + glitchOffset).r;
                float g = tex2D(_MainTex, pixelUV).g;
                float b = tex2D(_MainTex, pixelUV - glitchOffset).b;
                fixed4 color = fixed4(r, g, b, 1);

                float scanline = sin(i.uv.y * 200.0 + _Time.y * 10.0) * _ScanlineIntensity;
                color.rgb -= scanline;
                 
                float wave = sin(i.uv.y * 10.0 + _Time.y * 5.0) * 0.005 * _GlitchStrength;
                color.rgb = tex2D(_MainTex, pixelUV + float2(wave, 0)).rgb;

                //STATIC
                float2 uvTime = i.uv * _Time.y;  
                float dotProduct = dot(uvTime, float2(12.9898, 78.233));  
                float sineValue = sin(dotProduct);  
                float scaledValue = sineValue * 43758.5453;  
                float staticNoise = frac(scaledValue);  
                color.rgb += staticNoise * _StaticNoiseStrength; 

                return color;
            }
            ENDHLSL
        }
    }
}