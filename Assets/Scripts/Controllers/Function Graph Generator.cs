#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(EdgeCollider2D))]
public class FunctionGraphGenerator : MonoBehaviour
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
            ComputeFunctionGraph(() => { });
        }
    }

    private int kernelHandle = -1;
    private ComputeBuffer? encodedFunctionBuffer = null;
    private EdgeCollider2D? edgeCollider = null;

    public Matrix4x4 GetRenderTransform() => Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

    public void ComputeFunctionGraph(Action onCollisionDataReady)
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

        Vector2[] encodedFunction = ParseFunction();

        if (encodedFunctionBuffer == null || encodedFunctionBuffer.count != encodedFunction.Length)
        {
            encodedFunctionBuffer?.Release();
            encodedFunctionBuffer = new ComputeBuffer(encodedFunction.Length, sizeof(float) * 2);
        }

        encodedFunctionBuffer.SetData(encodedFunction);

        functionCalculatorComputeShader.SetBuffer(kernelHandle, "EncodedFunction", encodedFunctionBuffer);
        functionCalculatorComputeShader.SetFloat("BottomY", bottomY);
        functionCalculatorComputeShader.SetFloat("Resolution", resolution);
        functionCalculatorComputeShader.SetVector("Scale", new Vector2(transform.localScale.x, transform.localScale.y));
        functionCalculatorComputeShader.SetVector("Offset", new Vector2(transform.position.x, transform.position.z));

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

        functionCalculatorComputeShader.SetBuffer(kernelHandle, "SurfaceMeshVertices", VerticesBuffer);
        functionCalculatorComputeShader.SetBuffer(kernelHandle, "SurfaceMeshIndices", IndicesBuffer);
        functionCalculatorComputeShader.SetBuffer(kernelHandle, "CollisionEdgeVertices", CollisionEdgeBuffer);

        int threadGroups = Mathf.CeilToInt((float)resolution / 64);
        functionCalculatorComputeShader.Dispatch(kernelHandle, threadGroups, 1, 1);

        AsyncGPUReadback.Request(CollisionEdgeBuffer, (request) =>
        {
            if (request.hasError)
            {
                Debug.LogError("Error reading back CollisionEdgeBuffer data from GPU.");
                return;
            }

            Vector2[] collisionData = request.GetData<Vector2>().ToArray();

            if (edgeCollider != null)
            {
                edgeCollider.SetPoints(new List<Vector2>(collisionData));
            }

            onCollisionDataReady();
        });
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

    private float b = 0;

    private void Start()
    {
        // TODO: Убрать вызов из Start после отладки
        ComputeFunctionGraph(() =>
        {
            // Здесь можно использовать collisionData для дальнейших целей, например, для настройки коллайдеров
        });
    }

    private void Update()
    {
        if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
        {
            FunctionExpression = $"x*{b}";
            b += 0.001f;
        }
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

    private Vector2[] ParseFunction()
    {
        string infix = FunctionParser.RawStringToInfix(functionExpression);
        string rpn = FunctionParser.InfixToReversePolishNotation(infix);
        List<FunctionParser.Token> tokens = FunctionParser.TokenizeReversePolishNotation(rpn);
        List<Vector2> gpuTokens = FunctionParser.ConvertTokensToGPUTokens(tokens);
        gpuTokens.Insert(0, new Vector2(0, tokens.Count)); // Вставляем размер массива в начало
        return gpuTokens.ToArray();
    }
}