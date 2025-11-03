using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AxisLabels : MonoBehaviour
{
    public Camera cam;
    public TextMeshPro labelPrefab;
    public int range = 10;
    public float step = 1f;
    public float labelSize = 0.2f;

    private readonly List<TextMeshPro> labels = new();

    void Start()
    {
        for (int i = -range; i <= range; i++)
        {
            if (i == 0) continue;

            var lblX = Instantiate(labelPrefab, transform);
            lblX.text = i.ToString();
            lblX.fontSize = 10;
            lblX.transform.localScale = Vector3.one * labelSize;
            lblX.alignment = TextAlignmentOptions.Center;
            lblX.transform.localPosition = new Vector3(i * step - 0.2f, -0.08f, 0.9f);
            labels.Add(lblX);

            var lblY = Instantiate(labelPrefab, transform);
            lblY.text = i.ToString();
            lblY.fontSize = 10;
            lblY.transform.localScale = Vector3.one * labelSize;
            lblY.alignment = TextAlignmentOptions.Center;
            lblY.transform.localPosition = new Vector3(-0.2f, i * step, 0.9f);
            labels.Add(lblY);
        }
    }

    void LateUpdate()
    {
        if (!cam) return;

        var rotation = cam.transform.rotation;
        foreach (var label in labels)
            label.transform.rotation = rotation;
    }
}
