#nullable enable

using UnityEngine;

class GPULineRenderer : MonoBehaviour
{
    [Header("Rendering Settings")]
    public Material? lineMaterial = null;

    public ComputeBuffer? VerticesBuffer { get; set; } = null;

    public Matrix4x4 GetRenderTransform() => Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
}