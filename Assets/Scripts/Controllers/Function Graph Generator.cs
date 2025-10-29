#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FunctionGraphGenerator : MonoBehaviour
{
    [Header("Graph Settings")]
    public string functionExpression = "x+x*x";

    public void ComputeFunctionGraph()
    {
        List<Vector2> encodedFunction = ParseFunction();
    }

    private List<Vector2> ParseFunction()
    {
        string infix = FunctionParser.RawStringToInfix(functionExpression);
        string rpn = FunctionParser.InfixToReversePolishNotation(infix);
        List<FunctionParser.Token> tokens = FunctionParser.TokenizeReversePolishNotation(rpn);
        return FunctionParser.ConvertTokensToGPUTokens(tokens);
    }
}