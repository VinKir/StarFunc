using UnityEngine;
using UnityEngine.UI;
using CoreCLR.NCalc;
using TMPro;

public class BallTrajectory : MonoBehaviour
{
    public Transform ball;
    public TMP_InputField functionInput;
    public Button startButton;
    public float speed = 1f;

    private Expression expr;
    private bool validFormula = false;
    private bool moving = false;
    private float xValue = 0f;

    private void Start()
    {
        functionInput.onValueChanged.AddListener(OnFormulaChanged);
        startButton.onClick.AddListener(StartMoving);
    }

    private void OnFormulaChanged(string formula)
    {
        try
        {
            expr = new Expression(formula);
            expr.Parameters["x"] = 0f;

            object result = expr.Evaluate();
            float y = System.Convert.ToSingle(result);

            ball.position = new Vector3(0, y, 0);
            validFormula = true;
        }
        catch
        {
            ball.position = Vector3.zero;
            validFormula = false;
        }
    }

    private void StartMoving()
    {
        if (!validFormula) return;
        moving = true;
        xValue = 0f;
    }

    private void Update()
    {
        if (!moving) return;

        xValue += speed * Time.deltaTime;
        expr.Parameters["x"] = xValue;

        try
        {
            float y = System.Convert.ToSingle(expr.Evaluate());
            ball.position = new Vector3(xValue, y, 0);
        }
        catch
        {
            moving = false;
        }
    }
}
