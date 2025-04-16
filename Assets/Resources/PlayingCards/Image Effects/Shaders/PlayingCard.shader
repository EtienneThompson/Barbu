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
                float2 uv : TEXCOORD0;
                float3 viewNormal : TEXCOORD1;
            };

            sampler2D _FrontTex;
            sampler2D _BackTex;
            fixed4 _BorderColor;
            float _BorderThickness;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.viewNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.viewNormal));
                float frontFacing = dot(i.viewNormal, viewDir);
                float2 uv = i.uv;

                // Check if the pixel is within the border
                // Need extra offset for x because it is shorter than y.
                if (uv.x < 0.1675 + _BorderThickness || uv.x > 0.8325 - _BorderThickness ||
                    uv.y < _BorderThickness || uv.y > 1.0 - _BorderThickness)
                {
                    return _BorderColor; // Render the border color
                }

                // Render the front or back texture
                if (frontFacing > 0)
                {
                    return tex2D(_FrontTex, uv);
                }
                else
                {
                    return tex2D(_BackTex, uv);
                }
            }
            ENDCG
        }
    }
}
