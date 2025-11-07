Shader "Custom/TwoSidedCardWithBorder" {
    Properties {
        _FrontTex ("Front Texture", 2D) = "white" {}
        _BackTex ("Back Texture", 2D) = "white" {}
        _BorderColor ("Border Color", Color) = (1, 1, 1, 1)
        _BorderThickness ("Border Thickness", Range(0, 0.1)) = 0.05
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Cull Off // Disable backface culling to ensure both sides are rendered
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uvFront : TEXCOORD0;
                float2 uvBack : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            sampler2D _FrontTex;
            sampler2D _BackTex;
            float4 _FrontTex_ST;
            float4 _BackTex_ST;
            fixed4 _BorderColor;
            float _BorderThickness;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvFront = TRANSFORM_TEX(v.uv, _FrontTex);
                o.uvBack = TRANSFORM_TEX(v.uv, _BackTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float frontFacing = dot(i.worldNormal, viewDir);

                // Check if the pixel is within the border
                // Need extra offset for x because it is shorter than y.
                if (i.uvFront.x < 0.1675 + _BorderThickness || i.uvFront.x > 0.8325 - _BorderThickness ||
                    i.uvFront.y < _BorderThickness || i.uvFront.y > 1.0 - _BorderThickness)
                {
                    return _BorderColor; // Render the border color
                }

                // Sample appropriate texture
                if (frontFacing > 0)
                {
                    return tex2D(_FrontTex, i.uvFront);
                }
                else
                {
                    return tex2D(_BackTex, i.uvBack);
                }
            }
            ENDCG
        }
    }
}
