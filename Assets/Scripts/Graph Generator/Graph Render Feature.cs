#nullable enable

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GraphRenderFeature : ScriptableRendererFeature
{
    private readonly List<GraphRenderPass> renderPasses = new();

    public override void Create()
    {
        // Passes are created on-demand in AddRenderPasses
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        FunctionGraphGenerator[] generators = FindObjectsByType<FunctionGraphGenerator>(FindObjectsSortMode.None);

        // Ensure we have enough passes
        while (renderPasses.Count < generators.Length)
        {
            renderPasses.Add(new GraphRenderPass());
        }

        int passIndex = 0;
        foreach (var generator in generators)
        {
            // Validate all required components
            if (generator == null ||
                generator.VerticesBuffer == null ||
                !generator.VerticesBuffer.IsValid() ||
                generator.IndicesBuffer == null ||
                !generator.IndicesBuffer.IsValid() ||
                generator.graphMaterial == null ||
                generator.IndicesBuffer.count == 0)
            {
                continue;
            }

            var pass = renderPasses[passIndex];

            pass.Setup(
                generator.graphMaterial,
                generator.VerticesBuffer,
                generator.IndicesBuffer,
                generator.IndicesBuffer.count,
                generator.GetRenderTransform()
            );

            renderer.EnqueuePass(pass);
            passIndex++;
        }
    }
}
