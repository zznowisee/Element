Shader "Unlit/RoundLine"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Radius("Radius", Range(0.0, 1.0)) = 0.95
        _Antialias("Antialias", Range(0.001, 0.01)) = 0.001
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

            float4 _Color;
            float _Alpha;
            float _Radius;
            float _Antialias;

            float4 Circle(float2 centre, float2 pos, float radius, float4 color) {
                float dst = length(pos - centre);
                //float delta = fwidth(sqrDst);
                float a = 1 - smoothstep(radius - _Antialias, radius + _Antialias, dst);
                return float4(color.rgb, a);
            }

            float4 Line(float2 pos, float2 p1, float2 p2, float width, float4 color) {
                float k = (p1.y - p2.y) / (p1.x - p2.x);
                float b = p1.y - k * p1.x;
                float dst = abs(k * pos.x - pos.y + b) / sqrt(k * k + 1);
                float t = 1 - smoothstep(width - _Antialias, width + _Antialias, dst);
                return float4(color.rgb, t);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 centreOffset = (i.uv.xy - 0.5) * 2;
                float sqrDst = dot(centreOffset, centreOffset);
                float delta = fwidth(sqrt(sqrDst));
                float alpha = 1 - smoothstep(_Radius - delta, _Radius + delta, sqrDst);

                //return float4(_Color.xyz, alpha * _Color.a);
                float2 a = float2(0.3, 0.3);
                float2 b = float2(0.7, 0.7);
                float4 c1 = Circle(a, i.uv, _Radius, _Color);
                float4 c2 = Circle(b, i.uv, _Radius, _Color);
                float4 l = Line(i.uv, a, b, _Radius * 5, _Color);
                //float alpha = 
                return float4((c1 + c2).rgb, c1.a * c2.a * l.a);
            }
            ENDCG
        }
    }
}
