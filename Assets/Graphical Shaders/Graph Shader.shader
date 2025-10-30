Shader "Custom/GraphShader"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (1,1,1,1)
        _LineThickness ("Line Thickness", Float) = 0.1
        _LineIntensity ("Line Intensity", Range(0,10)) = 1.0
        _BaseColorTop ("Base Color Top", Color) = (0,0,1,1)
        _BaseColorBottom ("Base Color Bottom", Color) = (0,1,0,0.5)
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

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            StructuredBuffer<float3> Vertices;
            StructuredBuffer<uint> Indices;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float heightGradient : TEXCOORD1; // 0 = bottom, 1 = top
                float2 quadUV : TEXCOORD2; // x = horizontal position in quad, y = vertical (0-1)
                float isTopEdge : TEXCOORD3; // 1 if this vertex is on the top edge
                float3 topVertexPos : TEXCOORD4; // World position of the top vertex (for line rendering)
            };

            CBUFFER_START(UnityPerMaterial)
                float4x4 _ObjectToWorld;
                half4 _LineColor;
                half _LineThickness;
                half _LineIntensity;
                half4 _BaseColorTop;
                half4 _BaseColorBottom;
            CBUFFER_END

            Varyings Vertex(Attributes In)
            {
                Varyings OUT;

                uint index = Indices[In.vertexID];
                float3 pos = Vertices[index];

                float4 worldPos = mul(_ObjectToWorld, float4(pos, 1.0));
                OUT.positionHCS = mul(UNITY_MATRIX_VP, worldPos);
                OUT.worldPos = worldPos.xyz;
                
                // Determine if this is a top or bottom vertex
                // Bottom vertices have odd indices (1, 3, 5...), top vertices have even indices (0, 2, 4...)
                bool isBottomVertex = (index % 2) == 1;
                OUT.heightGradient = isBottomVertex ? 0.0 : 1.0;
                
                // Get the top vertex position for this vertex (used for line rendering)
                uint topVertexIndex = isBottomVertex ? (index - 1) : index;
                float3 topPos = Vertices[topVertexIndex];
                float4 topWorldPos = mul(_ObjectToWorld, float4(topPos, 1.0));
                OUT.topVertexPos = topWorldPos.xyz;
                
                // Calculate which triangle we're in and the vertex position within the quad
                uint triangleID = In.vertexID / 3;
                uint vertInTri = In.vertexID % 3;
                uint quadID = triangleID / 2;
                bool isFirstTriangle = (triangleID % 2) == 0;
                
                // Calculate UV coordinates for the quad
                // First triangle: v0(top-left), v2(top-right), v1(bottom-left)
                // Second triangle: v1(bottom-left), v2(top-right), v3(bottom-right)
                if (isFirstTriangle)
                {
                    if (vertInTri == 0) // v0 - top left
                    {
                        OUT.quadUV = float2(0.0, 1.0);
                        OUT.isTopEdge = 1.0;
                    }
                    else if (vertInTri == 1) // v2 - top right
                    {
                        OUT.quadUV = float2(1.0, 1.0);
                        OUT.isTopEdge = 1.0;
                    }
                    else // v1 - bottom left
                    {
                        OUT.quadUV = float2(0.0, 0.0);
                        OUT.isTopEdge = 0.0;
                    }
                }
                else
                {
                    if (vertInTri == 0) // v1 - bottom left
                    {
                        OUT.quadUV = float2(0.0, 0.0);
                        OUT.isTopEdge = 0.0;
                    }
                    else if (vertInTri == 1) // v2 - top right
                    {
                        OUT.quadUV = float2(1.0, 1.0);
                        OUT.isTopEdge = 1.0;
                    }
                    else // v3 - bottom right
                    {
                        OUT.quadUV = float2(1.0, 0.0);
                        OUT.isTopEdge = 0.0;
                    }
                }

                return OUT;
            }

            half4 Fragment(Varyings In) : SV_Target
            {
                // Base gradient color based on relative height within the quad
                half4 baseColor = lerp(_BaseColorBottom, _BaseColorTop, In.heightGradient);
                
                // Calculate screen-space distance to the interpolated top edge line
                // This creates a smooth line even with sparse vertices
                
                // Convert top vertex position to screen space
                float4 topClipPos = mul(UNITY_MATRIX_VP, float4(In.topVertexPos, 1.0));
                float2 topScreenPos = topClipPos.xy / topClipPos.w;
                
                // Current fragment in screen space
                float4 currentClipPos = mul(UNITY_MATRIX_VP, float4(In.worldPos, 1.0));
                float2 currentScreenPos = currentClipPos.xy / currentClipPos.w;
                
                // Calculate distance in screen space (Y axis only for horizontal lines)
                float screenDistY = abs(currentScreenPos.y - topScreenPos.y);
                
                // Convert line thickness to screen space
                // Adjust this multiplier to control line thickness sensitivity
                float lineThickness = _LineThickness * 0.001;
                
                // Create smooth anti-aliased line
                float lineWidth = fwidth(screenDistY) * _LineThickness * 0.5;
                float topEdgeMask = 1.0 - smoothstep(lineThickness - lineWidth, lineThickness + lineWidth, screenDistY);
                
                // Mix line color on the top edge
                half4 finalColor = lerp(baseColor, _LineColor, topEdgeMask) * _LineIntensity;

                return finalColor;
            }
            ENDHLSL
        }
    }
}