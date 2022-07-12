// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Gradient_3Color" {
    Properties{
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _ColorTop("Top Color", Color) = (1,1,1,1)
        _ColorMid("Mid Color", Color) = (1,1,1,1)
        _ColorBot("Bot Color", Color) = (1,1,1,1)
        _Middle("Middle", Range(0.001, 0.999)) = 1
        _Alpha("Alpha", Range(0.001, 0.999)) = 1
    }

        SubShader{
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100
        Fog {Mode Off}

            ZWrite On

            Blend SrcAlpha OneMinusSrcAlpha
                
            Pass {
            CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _ColorTop;
            fixed4 _ColorMid;
            fixed4 _ColorBot;
            float  _Middle;
            float _Alpha;

            struct v2f {
                float4 pos : SV_POSITION;
                float4 texcoord : TEXCOORD0;
            };

            v2f vert(appdata_full v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = float4(v.texcoord.xy, 0, 0);
                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                //_Middle = lerp(0.001, 0.999, unity_DeltaTime.x);
                fixed4 c = lerp(_ColorBot, _ColorMid, i.texcoord.x / _Middle) * step(i.texcoord.x, _Middle);
                c += lerp(_ColorMid, _ColorTop, (i.texcoord.x - _Middle) / (1 - _Middle)) * step(_Middle, i.texcoord.x);
                c.a = _Alpha;
                return c;
            }
            ENDCG
            }
        }
}