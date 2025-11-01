#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour, IDisposable
{
    [Header("Objects"), SerializeField]
    private ComputeShader? functionCalculatorComputeShader = null;
    [SerializeField]
    private GPULineRenderer? levelLine = null;
    [SerializeField]
    private GameObject? starPrefab = null;
    [SerializeField]
    private Transform? starParent = null;
    [SerializeField]
    private Transform? circleObject = null;
    [SerializeField]
    private Transform? cameraObject = null;

    [Header("Line parameters"), SerializeField]
    private int resolution = 1000;

    private ComputeBuffer? encodedFunctionBuffer = null;
    private ComputeBuffer? collisionEdgeBuffer = null;
    private int kernelHandle = -1;

    public void Dispose()
    {
        encodedFunctionBuffer?.Dispose();
        collisionEdgeBuffer?.Dispose();
    }

    //     public void AutomaticallyGenerateLevel(int hardness, int seed = -1)
    //     {
    //         // Hardness:
    //         // 1 - y = rx            linear function with random integer coefficient
    //         // 2 - y = r + r(x + r)  linear function with random integer coefficient + random integer constant
    //         // 3 - y = rx^2          quadratic function with random integer coefficient
    //         // 4 - y = r + rx^2      quadratic function with random integer coefficient + random integer constant
    //         // 5 - y = r + rx + rx^2 quadratic function with random integer coefficients + random integer constant
    //         // 6 - y = rx^3          cubic function with random integer coefficient

    //         if (hardness < 1 || hardness > 6)
    //         {
    //             Debug.LogError("LevelGenerator: Hardness level out of range (1-6).");
    //             return;
    //         }

    //         if (seed == -1)
    //         {
    //             seed = SettingsManager.Instance.CurrentSettings.seed;
    //         }

    //         UnityEngine.Random.InitState(seed);

    //         float[] rs = new float[5];

    //         float minR = -10;
    //         float maxR = 11;

    //         for (int i = 0; i < 5; i++)
    //         {
    //             rs[i] = UnityEngine.Random.Range(minR, maxR);
    //         }

    //         int starsCount = UnityEngine.Random.Range(3, 6);

    //         List<Vector3> positions = new();

    //         var verticalOffset = CalculateFunction(hardness, rs, 0);

    //         rs[0] -= verticalOffset;

    // #if UNITY_EDITOR

    //         Debug.Log($"Function is {OutputFunction(hardness, rs)}");

    // #endif

    //         for (int i = 0; i < resolution; i++)
    //         {
    //             float x = Mathf.Lerp(xStart, xEnd, (float)i / (resolution - 1));
    //             float y = CalculateFunction(hardness, rs, x);
    //             if (Mathf.Abs(y) > Mathf.Max(Mathf.Abs(xStart), Mathf.Abs(xEnd)))
    //             {
    //                 continue;
    //             }

    //             y = Mathf.Clamp(y, xStart, xEnd);
    //             positions.Add(new Vector3(x, y, 0f));
    //         }

    //         if (levelLine == null)
    //         {
    //             return;
    //         }

    //         levelLine.positionCount = positions.Count;
    //         levelLine.SetPositions(positions.ToArray());

    //         if (starPrefab == null || circleObject == null)
    //         {
    //             return;
    //         }
    //     }

    //     private float CalculateFunction(int hardness, float[] rs, float x)
    //     {
    //         return hardness switch
    //         {
    //             1 => rs[0] + rs[1] * x,
    //             2 => rs[0] + rs[1] * (x + rs[2]),
    //             3 => rs[0] + rs[1] * x * x,
    //             4 => rs[0] + rs[1] * (x + rs[2]) * (x + rs[2]),
    //             5 => rs[0] + rs[1] * x + rs[2] * x * x,
    //             6 => rs[0] + rs[1] * x * x * x,
    //             _ => 0f,
    //         };
    //     }

    // #if UNITY_EDITOR

    //     private string OutputFunction(int hardness, float[] rs)
    //     {
    //         return hardness switch
    //         {
    //             1 => $"{rs[0]} + {rs[1]} * x",
    //             2 => $"{rs[0]} + {rs[1]} * (x + {rs[2]})",
    //             3 => $"{rs[0]} + {rs[1]} * x * x",
    //             4 => $"{rs[0]} + {rs[1]} * (x * {rs[2]}) * (x * {rs[2]})",
    //             5 => $"{rs[0]} + {rs[1]} * x + {rs[2]} * x * x",
    //             6 => $"{rs[0]} + {rs[1]} * x * x * x",
    //             _ => "0"
    //         };
    //     }

    // #endif

    public void ManuallyGenerateLevel(string function, IEnumerable<Vector2> starPositions, Vector2 circlePosition)
    {
        if (levelLine == null)
        {
            // No drawing => no need to compute anything
            return;
        }

        if (functionCalculatorComputeShader == null)
        {
            Debug.LogError("Compute Shader not assigned!");
            return;
        }

        if (kernelHandle == -1)
        {
            kernelHandle = functionCalculatorComputeShader.FindKernel("FunctionCalculator");
        }

        Vector2[] encodedFunction = FunctionParser.ParseFunctionToGPUTokens(function);

        if (encodedFunctionBuffer == null || !encodedFunctionBuffer.IsValid() || encodedFunctionBuffer.count != encodedFunction.Length)
        {
            encodedFunctionBuffer?.Release();
            encodedFunctionBuffer = new ComputeBuffer(encodedFunction.Length, sizeof(float) * 2);
        }
        encodedFunctionBuffer.SetData(encodedFunction);

        int numCollisionVertices = resolution;

        if (collisionEdgeBuffer == null || !collisionEdgeBuffer.IsValid() || collisionEdgeBuffer.count != numCollisionVertices)
        {
            collisionEdgeBuffer?.Release();
            collisionEdgeBuffer = new ComputeBuffer(numCollisionVertices, sizeof(float) * 2);
        }

        FunctionCalculatorHandler.CalculateFunction(
            functionCalculatorComputeShader,
            kernelHandle,
            encodedFunctionBuffer,
            collisionEdgeBuffer,
            resolution,
            0f,
            new Vector2(levelLine.transform.localScale.x, levelLine.transform.localScale.y),
            new Vector2(levelLine.transform.position.x, levelLine.transform.position.z),
            verticesBuffer: null,
            indicesBuffer: null,
            scaleCollision: true,
            computeVertices: false,
            onCollisionDataReady: null
        );

        levelLine.VerticesBuffer = collisionEdgeBuffer;

        if (starPrefab != null && starParent != null)
        {
            foreach (var pos in starPositions)
            {

                Instantiate(starPrefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity, starParent);
            }
        }

        if (circleObject != null)
        {
            circleObject.position = new Vector3(circlePosition.x, circlePosition.y, circleObject.position.z);
        }

        if (cameraObject != null)
        {
            cameraObject.position = new Vector3(
                circlePosition.x,
                circlePosition.y - cameraObject.GetComponent<Camera>().orthographicSize * 0.75f,
                cameraObject.position.z
            );
        }
    }

    private void Awake()
    {
        if (functionCalculatorComputeShader == null)
        {
            Debug.LogError("LevelGenerator: Compute Shader not assigned!");
            return;
        }

        kernelHandle = functionCalculatorComputeShader.FindKernel("FunctionCalculator");
    }
}