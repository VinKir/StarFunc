Shader "Custom/LineShader"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (1,1,1,1)
        _LineThickness ("Line Thickness", Float) = 0.02
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
        }

        Pass
        {
            Cull Off
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            StructuredBuffer<float2> Vertices;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4x4 _ObjectToWorld;
                half4 _LineColor;
                half _LineThickness;
                int _VertexCount;
            CBUFFER_END

            Varyings Vertex(Attributes In)
            {
                Varyings OUT;

                // For MeshTopology.Triangles, we need to generate 6 vertices per line segment
                // to create 2 triangles forming a quad
                // Each segment: vertices [v0_left, v0_right, v1_left, v1_left, v0_right, v1_right]
                
                uint segmentIndex = In.vertexID / 6; // Which line segment
                uint vertInSegment = In.vertexID % 6; // Which vertex in this segment (0-5)
                
                // Determine which point and which side based on vertex pattern
                uint pointIndex;
                bool isRightSide;
                
                if (vertInSegment == 0) { pointIndex = segmentIndex; isRightSide = false; } // v0_left
                else if (vertInSegment == 1) { pointIndex = segmentIndex; isRightSide = true; } // v0_right
                else if (vertInSegment == 2) { pointIndex = segmentIndex + 1; isRightSide = false; } // v1_left
                else if (vertInSegment == 3) { pointIndex = segmentIndex + 1; isRightSide = false; } // v1_left (repeat)
                else if (vertInSegment == 4) { pointIndex = segmentIndex; isRightSide = true; } // v0_right (repeat)
                else { pointIndex = segmentIndex + 1; isRightSide = true; } // v1_right
                
                // Get current and next vertex positions for the segment
                float2 currentPos2D = Vertices[segmentIndex];
                float2 nextPos2D = (segmentIndex + 1 < _VertexCount) ? Vertices[segmentIndex + 1] : currentPos2D;
                
                // Clamp pointIndex to valid range
                pointIndex = min(pointIndex, _VertexCount - 1);
                float2 vertexPos2D = Vertices[pointIndex];
                
                float3 currentPos = float3(currentPos2D, 0);
                float3 nextPos = float3(nextPos2D, 0);
                float3 vertexPos = float3(vertexPos2D, 0);
                
                // Calculate line direction in world space
                float4 currentWorld = mul(_ObjectToWorld, float4(currentPos, 1.0));
                float4 nextWorld = mul(_ObjectToWorld, float4(nextPos, 1.0));
                float4 vertexWorld = mul(_ObjectToWorld, float4(vertexPos, 1.0));
                
                // Project to clip space
                float4 currentClip = mul(UNITY_MATRIX_VP, currentWorld);
                float4 nextClip = mul(UNITY_MATRIX_VP, nextWorld);
                float4 vertexClip = mul(UNITY_MATRIX_VP, vertexWorld);
                
                // Convert to screen space (NDC)
                float2 currentScreen = currentClip.xy / currentClip.w;
                float2 nextScreen = nextClip.xy / nextClip.w;
                
                // Calculate perpendicular direction in screen space
                float2 lineDir = normalize(nextScreen - currentScreen);
                float2 perpDir = float2(-lineDir.y, lineDir.x);
                
                // Expand the line by thickness in screen space
                float thickness = _LineThickness * 0.01; // Scale factor
                float2 offset = perpDir * thickness * (isRightSide ? 1.0 : -1.0);
                
                // Apply offset in clip space
                vertexClip.xy += offset * vertexClip.w;
                
                OUT.positionHCS = vertexClip;
                OUT.worldPos = vertexWorld.xyz;

                return OUT;
            }

            half4 Fragment(Varyings In) : SV_Target
            {
                return _LineColor;
            }
            
            ENDHLSL
        }
    }
}
