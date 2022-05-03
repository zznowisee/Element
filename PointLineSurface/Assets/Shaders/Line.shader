Shader "Unlit/Line"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Width("_Width", Range(0.0, 1.0)) = 1.0
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
            float _Width;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 centre = (i.uv.xy - 0.5) * 2;
                float sqrDst = centre.y * centre.y;
                float delta = fwidth(sqrt(sqrDst));
                float a = 1 - smoothstep(_Width - delta, _Width + delta, sqrDst);
                return fixed4(_Color.xyz,a * _Color.a);
            }
            ENDCG
        }
    }
}
