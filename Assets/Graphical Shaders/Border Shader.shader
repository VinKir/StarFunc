Shader "Custom/BorderShader"
{
    Properties
    {
        _StripesSize ("Stripes Size", Float) = 10.0
        _StripesAngle ("Stripes Angle", Range(0, 360)) = 45.0
        _StripesColor ("Stripes Color", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (1,0,0,1)
        _BorderIntensity ("Border Intensity", Range(0,10)) = 1.0
        _BorderThickness ("Border Thickness", Float) = 0.1
        // 0 = Left, 1 = Top, 2 = Right, 3 = Bottom
        _BorderDirection ("Border Direction", int) = 1
        _BaseColor ("Base Color", Color) = (0,0,1,1)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline" = "UniversalPipeline" 
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back

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
                float3 worldPos : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float _StripesSize;
                float _StripesAngle;
                half4 _StripesColor;
                half4 _BorderColor;
                half _BorderIntensity;
                half _BorderThickness;
                int _BorderDirection;
                half4 _BaseColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Start with base color
                half4 color = _BaseColor;

                // Calculate distance from the border edge based on direction (using UV for border detection)
                float distFromBorder = 0.0;

                // Use world-space position for stripe pattern (uniform tiling)
                float2 worldCoord = IN.worldPos.xy; // Use XZ plane for horizontal surfaces

                // _BorderDirection: 0 = Left, 1 = Top, 2 = Right, 3 = Bottom
                if (_BorderDirection == 0) // Left
                {
                    distFromBorder = IN.uv.x;
                }
                else if (_BorderDirection == 1) // Top
                {
                    distFromBorder = 1.0 - IN.uv.y;
                }
                else if (_BorderDirection == 2) // Right
                {
                    distFromBorder = 1.0 - IN.uv.x;
                }
                else // Bottom (_BorderDirection == 3)
                {
                    distFromBorder = IN.uv.y;
                }

                // Create rotated stripes pattern using world-space coordinates
                // Rotate the coordinate system by _StripesAngle
                float angleRad = radians(_StripesAngle);
                float cosAngle = cos(angleRad);
                float sinAngle = sin(angleRad);

                // Apply rotation matrix to world coordinates (XZ plane)
                // This rotates the stripe direction
                float rotatedCoord = worldCoord.x * cosAngle - worldCoord.y * sinAngle;

                float stripePattern = frac(rotatedCoord * _StripesSize);
                float stripesMask = step(0.5, stripePattern) * (1 - distFromBorder);

                color *= (1 - distFromBorder);

                // Apply stripes color where pattern is active
                color = lerp(color, _StripesColor, stripesMask);
                
                // Calculate border line intensity
                float borderEdge = _BorderThickness * 0.1;
                float borderFade = fwidth(distFromBorder) * 2.0;
                float borderMask = 1.0 - smoothstep(borderEdge - borderFade, borderEdge + borderFade, distFromBorder);

                // Apply border color with intensity
                half4 borderColorIntense = _BorderColor * _BorderIntensity;
                color = lerp(color, borderColorIntense, borderMask);

                return color;
            }
            ENDHLSL
        }
    }
}
