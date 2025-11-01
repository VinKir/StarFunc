#nullable enable

using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

class FunctionCalculatorHandler
{
    public static void CalculateFunction(
        ComputeShader computeShader,
        int kernelHandle,
        ComputeBuffer encodedFunctionBuffer,
        ComputeBuffer collisionEdgeBuffer,
        int resolution,
        float bottomY,
        Vector2 scale,
        Vector2 offset,
        ComputeBuffer? verticesBuffer = null,
        ComputeBuffer? indicesBuffer = null,
        bool scaleCollision = false,
        bool computeVertices = true,
        System.Action<NativeArray<Vector2>>? onCollisionDataReady = null)
    {
        if (kernelHandle == -1)
        {
            Debug.LogError("FunctionCalculatorHandler: Invalid kernel handle.");
            return;
        }

        bool verticesCreatedInHandler = verticesBuffer == null;
        bool indicesCreatedInHandler = indicesBuffer == null;

        if (verticesCreatedInHandler)
        {
            verticesBuffer = new ComputeBuffer(1, sizeof(float) * 3);
        }

        if (indicesCreatedInHandler)
        {
            indicesBuffer = new ComputeBuffer(1, sizeof(int));
        }

        computeShader.SetBuffer(kernelHandle, "EncodedFunction", encodedFunctionBuffer);
        computeShader.SetInt("Resolution", resolution);
        computeShader.SetFloat("BottomY", bottomY);
        computeShader.SetBool("ComputeVertices", computeVertices);
        computeShader.SetVector("Scale", scale);
        computeShader.SetVector("Offset", offset);
        computeShader.SetBool("ScaleCollision", scaleCollision);

        computeShader.SetBuffer(kernelHandle, "CollisionEdgeVertices", collisionEdgeBuffer);
        computeShader.SetBuffer(kernelHandle, "SurfaceMeshVertices", verticesBuffer);
        computeShader.SetBuffer(kernelHandle, "SurfaceMeshIndices", indicesBuffer);

        int threadGroups = Mathf.CeilToInt(resolution / 64f);
        computeShader.Dispatch(kernelHandle, threadGroups, 1, 1);

        if (verticesCreatedInHandler && verticesBuffer != null)
        {
            verticesBuffer.Dispose();
        }

        if (indicesCreatedInHandler && indicesBuffer != null)
        {
            indicesBuffer.Dispose();
        }

        if (onCollisionDataReady == null)
        {
            return;
        }

        AsyncGPUReadback.Request(collisionEdgeBuffer, (request) =>
        {
            if (request.hasError)
            {
                Debug.LogError("Error reading back CollisionEdgeBuffer data from GPU.");
                return;
            }

            var collisionData = request.GetData<Vector2>();
            onCollisionDataReady?.Invoke(collisionData);
        });
    }
}