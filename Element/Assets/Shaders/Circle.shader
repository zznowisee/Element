Shader "Unlit/Circle"
{
    Properties
    {
        _innerRadius("_innerRadius", Range(0.0, 0.5)) = 0
        _outterRadius("_outterRadius", Range(0.5, 1.0)) = 0.5
        _Color("_Color", Color) = (1,1,1,1)
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

            float _innerRadius;
            float _outterRadius;
            fixed4 _Color;

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
                float sqrDst = dot(centre, centre);
                float delta = fwidth(sqrt(sqrDst));

                float innerAlpha = smoothstep(_innerRadius - delta, _innerRadius + delta, sqrDst);
                float outterAlpha = smoothstep(_outterRadius - delta, _outterRadius + delta, sqrDst);
                return fixed4(_Color.xyz, innerAlpha - outterAlpha);
            }
            ENDCG
        }
    }
}
