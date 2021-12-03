Shader "Unlit/Shape"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Alpha("Alpha",Range(0.0,1.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define PI 3.14159265359
            #define TWO_PI 6.28318530718

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

            float4 _Color;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float N = 2.0;
                float2 pos = i.uv * 2.0 - 1.0;
                float angle = atan(pos.x / pos.y) + PI;
                float r = TWO_PI / N;

                float d = cos(floor(0.5 + angle / r) * r - angle) * length(pos);
                float delta = fwidth(d);
                float a = 1 - smoothstep(0.8, 0.81, d);

                return fixed4(_Color.xyz, a);
            }
            ENDCG
        }
    }
}
