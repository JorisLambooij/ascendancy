Shader "Unlit/FOW_Shader"
{
    Properties
    {
        _VisibilityTex("Visiblity", 2D) = "black" {}
        _DiscoveryTex("Discovery", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _DiscoveryTex;
            sampler2D _VisibilityTex;
            float4 _DiscoveryTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _DiscoveryTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 dCol = tex2D(_DiscoveryTex, i.uv);
                fixed4 vCol = tex2D(_VisibilityTex, i.uv);
                fixed4 combinedCol = max(dCol, vCol);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, combinedCol);
                return combinedCol;
            }
            ENDCG
        }
    }
}
