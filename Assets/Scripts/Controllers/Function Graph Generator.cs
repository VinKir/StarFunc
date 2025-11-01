#nullable enable

using System;
using System.Collections.Generic;
using System.Data;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(EdgeCollider2D))]
public class FunctionGraphGenerator : MonoBehaviour, IDisposable
{
    [Header("Compute Shader Settings")]
    public ComputeShader? functionCalculatorComputeShader = null;

    [Header("Graph Settings"), SerializeField]
    private string functionExpression = "x*x";
    [SerializeField]
    private float bottomY = -5f;
    [Min(64)]
    [SerializeField]
    private int resolution = 256;

    [Header("Rendering Settings")]
    public Material? graphMaterial = null;

    public ComputeBuffer? VerticesBuffer { get; set; } = null;
    public ComputeBuffer? IndicesBuffer { get; set; } = null;
    public ComputeBuffer? CollisionEdgeBuffer { get; set; } = null;
    public string FunctionExpression
    {
        get => functionExpression;
        set
        {
            functionExpression = value;
            ComputeFunctionGraph(ForceCollisionUpdate);
        }
    }
    public bool ForceCollisionUpdate { get; set; } = true;

    private int kernelHandle = -1;
    private ComputeBuffer? encodedFunctionBuffer = null;
    private EdgeCollider2D? edgeCollider = null;

    public Matrix4x4 GetRenderTransform() => Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

    public void Dispose()
    {
        ReleaseBuffers();
        VerticesBuffer?.Dispose();
        IndicesBuffer?.Dispose();
        CollisionEdgeBuffer?.Dispose();
        encodedFunctionBuffer?.Dispose();
    }

    public void ComputeFunctionGraph(bool retrieveCollision, Action? onCollisionDataReady = null)
    {
        if (functionCalculatorComputeShader == null)
        {
            Debug.LogError("Compute Shader not assigned!");
            return;
        }

        if (kernelHandle == -1)
        {
            kernelHandle = functionCalculatorComputeShader.FindKernel("FunctionCalculator");
        }

        Vector2[] encodedFunction = FunctionParser.ParseFunctionToGPUTokens(functionExpression);

        if (encodedFunctionBuffer == null || encodedFunctionBuffer.count != encodedFunction.Length)
        {
            encodedFunctionBuffer?.Release();
            encodedFunctionBuffer = new ComputeBuffer(encodedFunction.Length, sizeof(float) * 2);
        }

        encodedFunctionBuffer.SetData(encodedFunction);

        int numVertices = resolution * 2;
        int numIndices = (resolution - 1) * 6; // (resolution - 1) quads, each quad = 6 indices
        int numCollisionVertices = resolution;

        if (VerticesBuffer == null || VerticesBuffer.count != numVertices)
        {
            VerticesBuffer?.Release();
            VerticesBuffer = new ComputeBuffer(numVertices, sizeof(float) * 3);
        }

        if (IndicesBuffer == null || IndicesBuffer.count != numIndices)
        {
            IndicesBuffer?.Release();
            IndicesBuffer = new ComputeBuffer(numIndices, sizeof(int));
        }

        if (CollisionEdgeBuffer == null || CollisionEdgeBuffer.count != numCollisionVertices)
        {
            CollisionEdgeBuffer?.Release();
            CollisionEdgeBuffer = new ComputeBuffer(numCollisionVertices, sizeof(float) * 2);
        }

        FunctionCalculatorHandler.CalculateFunction(
            functionCalculatorComputeShader,
            kernelHandle,
            encodedFunctionBuffer,
            CollisionEdgeBuffer,
            resolution,
            bottomY,
            new Vector2(transform.localScale.x, transform.localScale.y),
            new Vector2(transform.position.x, transform.position.z),
            VerticesBuffer,
            IndicesBuffer,
            false,
            graphMaterial != null,
            retrieveCollision ? collisionData =>
            {
                if (edgeCollider == null)
                {
                    edgeCollider = GetComponent<EdgeCollider2D>();
                }
                edgeCollider.SetPoints(new List<Vector2>(collisionData));

                onCollisionDataReady?.Invoke();
            }
        : null
        );
    }

    private void OnValidate()
    {
        // Resolution должно делиться на 64 без остатка
        if (resolution % 64 != 0)
        {
            resolution = Mathf.CeilToInt((float)resolution / 64) * 64;
        }
    }

    private void Awake()
    {
        if (functionCalculatorComputeShader == null)
        {
            Debug.LogError("Compute Shader not assigned!");
            return;
        }

        kernelHandle = functionCalculatorComputeShader.FindKernel("FunctionCalculator");
        edgeCollider = GetComponent<EdgeCollider2D>();
    }

    private void OnDestroy()
    {
        ReleaseBuffers();
    }

    private void OnDisable()
    {
        ReleaseBuffers();
    }

    private void ReleaseBuffers()
    {
        encodedFunctionBuffer?.Release();
        VerticesBuffer?.Release();
        IndicesBuffer?.Release();

        encodedFunctionBuffer = null;
        VerticesBuffer = null;
        IndicesBuffer = null;
    }
}