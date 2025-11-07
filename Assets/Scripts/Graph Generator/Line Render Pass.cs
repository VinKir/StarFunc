#nullable enable

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class LineRenderPass : ScriptableRenderPass
{
    private class PassData
    {
        internal Material? material;
        internal MaterialPropertyBlock? propertyBlock;
        internal ComputeBuffer? verticesBuffer;
        internal Matrix4x4 objectToWorld;
    }

    private readonly PassData passData;
    private readonly MaterialPropertyBlock materialPropertyBlock;

    public LineRenderPass()
    {
        passData = new PassData();
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    public void Setup(
        Material mat,
        ComputeBuffer vertices,
        Matrix4x4 transform)
    {
        // Set render pass event based on material's render queue
        if (mat != null)
        {
            int queue = mat.renderQueue;

            if (queue < 1000) // Before Background
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            }
            else if (queue <= 1500) // Background (1000)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            }
            else if (queue <= 2000) // Geometry (2000)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            }
            else if (queue <= 2450) // AlphaTest (2450)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            }
            else if (queue < 3000) // AlphaBlend (2500)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            }
            else if (queue <= 3500) // Transparent (3000)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            }
            else // Overlay (4000+)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            }
        }
        // Validate buffers before setting up
        if (vertices == null || !vertices.IsValid())
        {
            Debug.LogWarning("LineRenderPass.Setup: Invalid or null buffers provided");
            passData.material = null;
            return;
        }

        materialPropertyBlock.Clear();
        materialPropertyBlock.SetBuffer("Vertices", vertices);
        materialPropertyBlock.SetMatrix("_ObjectToWorld", transform);
        materialPropertyBlock.SetInt("_VertexCount", vertices.count);

        passData.material = mat;
        passData.propertyBlock = materialPropertyBlock;
        passData.verticesBuffer = vertices;
        passData.objectToWorld = transform;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (passData.material == null ||
            passData.verticesBuffer == null ||
            !passData.verticesBuffer.IsValid())
        {
            return;
        }

        using var builder = renderGraph.AddRasterRenderPass<PassData>("LineRenderPass", out var passDataHandle);

        // Get the camera color target from the frame data
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
        builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);

        // Copy pass data
        passDataHandle.material = passData.material;
        passDataHandle.propertyBlock = passData.propertyBlock;
        passDataHandle.verticesBuffer = passData.verticesBuffer;

        builder.AllowPassCulling(false);

        builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
        {
            if (data.material == null || data.propertyBlock == null || data.verticesBuffer == null)
            {
                Debug.LogWarning("LineRenderPass: Material or PropertyBlock or VerticesBuffer is null during rendering.");
                return;
            }

            // For thickness-expanded lines, we need 2 vertices per line point
            // DrawProcedural doesn't support TriangleStrip in some Unity versions
            // So we need to generate proper triangle indices
            // Each segment needs 2 triangles (6 vertices in triangle mode)
            int segmentCount = data.verticesBuffer.count - 1;
            int triangleVertexCount = segmentCount * 6; // 2 triangles per segment, 3 vertices per triangle

            context.cmd.DrawProcedural(
                Matrix4x4.identity,
                data.material,
                0,
                MeshTopology.Triangles,
                triangleVertexCount,
                1,
                data.propertyBlock
            );
        });
    }
}
