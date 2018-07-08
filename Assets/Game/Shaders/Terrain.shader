Shader "Custom/Terrain" 
{
	SubShader 
    {
		Tags { "RenderType"="Opaque" }
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(v.normal, unity_WorldToObject);
                o.uv = v.uv;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_TARGET
            {
                return fixed4(i.normal * 0.5 + 0.5, 1.0);
            }

            ENDCG
        }
	}
}
