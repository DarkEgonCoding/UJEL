Shader "Hidden/DayCycleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
	    float _Intensity;
            float _AnimProgress;
	    fixed4 _BlendColor;

            fixed4 frag (v2f i) : SV_Target
            {
                // The first shader
                fixed4 col = tex2D(_MainTex, i.uv);
                return col.xyzw * _BlendColor.xyzw * _Intensity;

                // Cave Shader
//                fixed4 col;
//                float radius = 0.2;
//                fixed2 pos = fixed2(i.uv.x, i.uv.y) - fixed2(0.5, 0.5);
//                float dist = sqrt(pos.x * pos.x + pos.y * pos.y);
//
//                if (dist > radius) {
//                    col = float4(0.0, 0.0, 0.0, 1.0);
//                } else {
//                    col = tex2D(_MainTex, i.uv);
//                }
//
//                return col;
            }
            ENDCG
        }
    }
}
