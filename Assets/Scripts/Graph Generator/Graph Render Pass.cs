#nullable enable

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class GraphRenderPass : ScriptableRenderPass
{
    private class PassData
    {
        internal Material? material;
        internal MaterialPropertyBlock? propertyBlock;
        internal ComputeBuffer? verticesBuffer;
        internal ComputeBuffer? indicesBuffer;
        internal int indicesCount;
        internal Matrix4x4 objectToWorld;
    }

    private readonly PassData passData;
    private readonly MaterialPropertyBlock materialPropertyBlock;

    public GraphRenderPass()
    {
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        passData = new PassData();
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    public void Setup(
        Material mat,
        ComputeBuffer vertices,
        ComputeBuffer indices,
        int indexCount,
        Matrix4x4 transform)
    {
        // Validate buffers before setting up
        if (vertices == null || !vertices.IsValid() || indices == null || !indices.IsValid())
        {
            Debug.LogWarning("GraphRenderPass.Setup: Invalid or null buffers provided");
            passData.material = null;
            return;
        }

        materialPropertyBlock.Clear();
        materialPropertyBlock.SetBuffer("Vertices", vertices);
        materialPropertyBlock.SetBuffer("Indices", indices);
        materialPropertyBlock.SetMatrix("_ObjectToWorld", transform);

        passData.material = mat;
        passData.propertyBlock = materialPropertyBlock;
        passData.verticesBuffer = vertices;
        passData.indicesBuffer = indices;
        passData.indicesCount = indexCount;
        passData.objectToWorld = transform;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (passData.material == null ||
            passData.verticesBuffer == null ||
            !passData.verticesBuffer.IsValid() ||
            passData.indicesBuffer == null ||
            !passData.indicesBuffer.IsValid())
        {
            return;
        }

        using var builder = renderGraph.AddRasterRenderPass<PassData>("GraphRenderPass", out var passDataHandle);

        // Get the camera color target from the frame data
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
        builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);

        // Copy pass data
        passDataHandle.material = passData.material;
        passDataHandle.propertyBlock = passData.propertyBlock;
        passDataHandle.indicesCount = passData.indicesCount;

        builder.AllowPassCulling(false);

        builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
        {
            if (data.material == null || data.propertyBlock == null)
            {
                Debug.LogWarning("GraphRenderPass: Material or PropertyBlock is null during rendering.");
                return;
            }

            if (data.indicesCount == 0)
            {
                Debug.LogWarning("GraphRenderPass: Indices count is zero, skipping draw.");
                return;
            }

            context.cmd.DrawProcedural(
                Matrix4x4.identity,
                data.material,
                0,
                MeshTopology.Triangles,
                data.indicesCount,
                1,
                data.propertyBlock
            );
        });
    }
}
