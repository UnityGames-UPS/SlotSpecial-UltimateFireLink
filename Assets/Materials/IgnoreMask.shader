Shader "UI/IgnoreUIMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ColorMask ("Color mask", Int) = 15
        _Stencil ("Stencil Mask", Float) = 1.0
        _StencilComp ("Stencil Comparison", Int) = 8 
        _StencilOp ("Stencil Operation", Int) = 0
        _StencilReadMask ("Stencil Read Mask", Int) = 255 
        _StencilWriteMask ("Stencil Write Mask", Int) = 255 

    }
    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Stencil
        {
            Ref 0
            Comp Always
            Pass Replace
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;

                if (texColor.a < 0.01)
                    discard;

                return texColor;
            }
            ENDCG
        }
    }
    Fallback "Transparent"
}