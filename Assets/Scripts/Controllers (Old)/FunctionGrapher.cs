using UnityEngine;
using TMPro;
using System.Collections.Generic;
using CoreCLR.NCalc;

[RequireComponent(typeof(LineRenderer))]
public class FunctionGrapher : MonoBehaviour
{
    public TMP_InputField inputField;
    public int length = 100;
    public float scale = 1f;
    public float minX = -100f;
    public float maxX = 100f;
    public float step = 0.1f;

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCol;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawGraph()
    {
        string funcText = inputField.text;
        Expression expr = new Expression(funcText);

        List<Vector2> points = new List<Vector2>();

        for (float x = minX; x <= maxX; x += step)
        {
            expr.Parameters["x"] = x;
            float y = System.Convert.ToSingle(expr.Evaluate());
            points.Add(new Vector2(x, y) * scale);
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ConvertAll(v => (Vector3)v).ToArray());

        GenerateCollider(points);
    }

    private void GenerateCollider(List<Vector2> points)
    {
        if (edgeCol != null)
            Destroy(edgeCol);

        edgeCol = gameObject.AddComponent<EdgeCollider2D>();
        edgeCol.points = points.ToArray();
    }
}
