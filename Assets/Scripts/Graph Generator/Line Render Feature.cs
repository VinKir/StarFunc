#nullable enable

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LineRenderFeature : ScriptableRendererFeature
{
    private readonly List<LineRenderPass> renderPasses = new();

    public override void Create()
    {
        // Passes are created on-demand in AddRenderPasses
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        GPULineRenderer[] generators = FindObjectsByType<GPULineRenderer>(FindObjectsSortMode.None);

        // Ensure we have enough passes
        while (renderPasses.Count < generators.Length)
        {
            renderPasses.Add(new LineRenderPass());
        }

        int passIndex = 0;
        foreach (var generator in generators)
        {
            // Validate all required components
            if (generator == null ||
                generator.VerticesBuffer == null ||
                !generator.VerticesBuffer.IsValid() ||
                generator.lineMaterial == null)
            {
                continue;
            }

            var pass = renderPasses[passIndex];

            pass.Setup(
                generator.lineMaterial,
                generator.VerticesBuffer,
                generator.GetRenderTransform()
            );

            renderer.EnqueuePass(pass);
            passIndex++;
        }
    }
}
