Shader "Unlit/Hex"
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float delta = fwidth(i.uv.x);
                float a = smoothstep(_Outter - delta, _Outter + delta, i.uv.x);
                return fixed4(_Color.xyz,a);
            }
            ENDCG
        }
    }
}
