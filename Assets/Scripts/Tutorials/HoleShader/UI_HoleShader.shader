Shader "CustomRenderTexture/UI_HoleShader"
{
    Properties
    {
        _Color ("Overlay Color", Color) = (0,0,0,0.75)
        _FocusCenter ("Focus Center (UV)", Vector) = (0.5,0.5,0,0)
        _FocusSize ("Focus Size (UV)", Vector) = (0.3,0.2,0,0)
        _CornerRadius ("Corner Radius", Float) = 0.02
        _Softness ("Edge Softness", Float) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float4 _FocusCenter;
            float4 _FocusSize;
            float _CornerRadius;
            float _Softness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float RoundedRectMask(float2 uv)
            {
                float2 halfSize = _FocusSize.xy * 0.5;
                float2 d = abs(uv - _FocusCenter.xy) - halfSize + _CornerRadius;
                float outside = length(max(d, 0.0)) - _CornerRadius;
                return smoothstep(0.0, _Softness, outside);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float mask = RoundedRectMask(i.uv);
                fixed4 col = _Color;
                col.a *= mask;
                return col;
            }
            ENDCG
        }
    }
}
