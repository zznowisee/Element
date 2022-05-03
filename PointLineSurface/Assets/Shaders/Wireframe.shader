Shader "Unlit/Wireframe"
{
    Properties
    {
        _Color("_Color", Color) = (1,1,1,1)
        _Outter("_Outter", Range(0.9,1)) = 0.98
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

            fixed4 _Color;
            float _Outter;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //float delta = fwidth(i.uv.xy);
                //float ax = smoothstep(_Outter - delta, _Outter + delta, i.uv.x) + (1 - smoothstep((1 - _Outter) - delta, (1 - _Outter) + delta, i.uv.x));
                //float ay = smoothstep(_Outter - delta, _Outter + delta, i.uv.y) + (1 - smoothstep((1 - _Outter) - delta, (1 - _Outter) + delta, i.uv.y));
                float ax = step(_Outter, i.uv.x) + (1 - step(1 - _Outter, i.uv.x));
                float ay = step(_Outter, i.uv.y) + (1 - step(1 - _Outter, i.uv.y));
                return fixed4(_Color.xyz, ax + ay);
            }
            ENDCG
        }
    }
}