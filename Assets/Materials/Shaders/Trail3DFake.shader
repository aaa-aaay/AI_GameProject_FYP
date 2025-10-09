Shader "Custom/TrailRoundBeam"
{
    Properties
    {
        _Color ("Main Color", Color) = (0,1,1,1)
        _Intensity ("Glow Intensity", Range(0,5)) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Distance from center of the trail (v = 0.5 is middle)
                float dist = abs(i.uv.y - 0.5) * 2.0;

                // Simulate a circular cross-section
                float circle = saturate(1.0 - dist * dist);

                fixed4 col = _Color;
                col.rgb *= circle * _Intensity;
                col.a   *= circle;

                return col;
            }
            ENDCG
        }
    }
}
