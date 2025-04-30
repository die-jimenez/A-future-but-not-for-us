Shader "URP/CircleEdges"
{
    Properties
    {
        _Color ("Color", Color) = (0.5059, 0.0667, 0.6235, 1)  // Color morado #81119F
        _BorderThickness ("Border Thickness", Float) = 0.05
        _Radius ("Radius", Float) = 0.4
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;  // El color del borde
            float _BorderThickness;
            float _Radius;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 center = float2(0.5, 0.5);

                // Coordenadas UV ya están normalizadas para el SpriteRenderer
                float2 diff = input.uv - center;
                float distance = sqrt(diff.x * diff.x + diff.y * diff.y);

                // Calcular el borde sin suavizado
                float alpha = 0.0;
                if (distance >= _Radius && distance <= (_Radius + _BorderThickness))
                {
                    alpha = 1.0;  // Totalmente visible en el borde
                }

                // Asegurarse de que el borde tenga el color deseado
                return half4(_Color.rgb, alpha * _Color.a);
            }
            ENDHLSL
        }
    }
}

