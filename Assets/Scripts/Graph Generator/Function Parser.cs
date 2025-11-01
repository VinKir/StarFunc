#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Парсер математических функций.
/// Преобразует строковое представление функции в токены для вычисления на GPU.
/// Поддерживает инфиксную нотацию и обратную польскую нотацию (RPN).
/// </summary>
public class FunctionParser
{
    /// <summary>
    /// Токен - элементарная единица математического выражения.
    /// Может быть числом, оператором или переменной.
    /// </summary>
    public struct Token
    {
        /// <summary>
        /// Тип токена.
        /// </summary>
        public enum TokenType
        {
            Number,    // Числовое значение (например, 3.14)
            Operator,  // Математический оператор или функция (например, +, sin)
            Variable   // Переменная (например, x)
        }

        /// <summary>
        /// Тип текущего токена.
        /// </summary>
        public TokenType Type;

        /// <summary>
        /// Строковое значение токена.
        /// </summary>
        public string Value;

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Преобразует токен в формат для GPU (Vector2).
        /// X компонент - тип токена (0=число, 1=оператор, 2=переменная).
        /// Y компонент - значение или код операции.
        /// </summary>
        /// <returns>Vector2 для передачи в GPU шейдер.</returns>
        public readonly Vector2 GetGPUToken()
        {
            return Type switch
            {
                // Для чисел: тип=0, значение=само число
                TokenType.Number => new Vector2(1, float.Parse(Value)),

                // Для операторов: тип=1, значение=код операции
                TokenType.Operator => new Vector2(2, Value switch
                {
                    "+" => 0,    // Сложение
                    "-" => 1,    // Вычитание
                    "*" => 2,    // Умножение
                    "/" => 3,    // Деление
                    "^" => 4,    // Возведение в степень
                    "sin" => 5,  // Синус
                    "cos" => 6,  // Косинус
                    "tan" => 7,  // Тангенс
                    "log" => 8,  // Логарифм
                    "exp" => 9,  // Экспонента
                    _ => throw new Exception("Unknown operator")
                }),

                // Для переменных: тип=2, значение=0
                TokenType.Variable => new Vector2(3, 0),

                _ => throw new Exception("Unknown token type")
            };
        }
    }

    public static Vector2[] ParseFunctionToGPUTokens(string functionExpression)
    {
        string infix = RawStringToInfix(functionExpression);
        string rpn = InfixToReversePolishNotation(infix);
        List<Token> tokens = TokenizeReversePolishNotation(rpn);
        List<Vector2> gpuTokens = ConvertTokensToGPUTokens(tokens);
        gpuTokens.Insert(0, new Vector2(0, tokens.Count)); // Вставляем размер массива в начало
        return gpuTokens.ToArray();
    }

    /// <summary>
    /// Преобразует список токенов в список векторов для GPU.
    /// </summary>
    /// <param name="tokens">Список токенов.</param>
    /// <returns>Список Vector2 для передачи в GPU шейдер.</returns>
    public static List<Vector2> ConvertTokensToGPUTokens(List<Token> tokens) => tokens.Select(token => token.GetGPUToken()).ToList();

    /// <summary>
    /// Преобразует сырую строку функции в инфиксную нотацию с пробелами.
    /// Например: "sin(x)+2*x" -> "sin ( x ) + 2 * x"
    /// </summary>
    /// <param name="raw">Сырая строка функции без пробелов.</param>
    /// <returns>Инфиксная нотация с пробелами между токенами.</returns>
    public static string RawStringToInfix(string raw)
    {
        // Добавляем пробелы вокруг операторов, скобок и функций
        string[] operators = { "+", "-", "*", "/", "^", "(", ")", "sin", "cos", "tan", "log", "exp" };
        foreach (string op in operators)
        {
            raw = raw.Replace(op, $" {op} ");
        }

        // Удаляем лишние пробелы и объединяем токены одним пробелом
        raw = string.Join(' ', raw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        return raw;
    }

    /// <summary>
    /// Преобразует инфиксную нотацию в обратную польскую нотацию (RPN).
    /// Использует алгоритм сортировочной станции Дейкстры.
    /// Например: "2 + 3 * 4" -> "2 3 4 * +"
    /// </summary>
    /// <param name="infix">Выражение в инфиксной нотации (с пробелами).</param>
    /// <returns>Выражение в обратной польской нотации.</returns>
    public static string InfixToReversePolishNotation(string infix)
    {
        // Таблица приоритетов операторов
        Dictionary<string, int> precedence = new()
        {
            { "+", 1 },    // Сложение и вычитание - низший приоритет
            { "-", 1 },
            { "*", 2 },    // Умножение и деление - средний приоритет
            { "/", 2 },
            { "^", 3 },    // Возведение в степень - высокий приоритет
            { "sin", 4 },  // Функции - наивысший приоритет
            { "cos", 4 },
            { "tan", 4 },
            { "log", 4 },
            { "exp", 4 }
        };

        Stack<string> operators = new();  // Стек для операторов
        List<string> output = new();      // Выходная очередь

        string[] tokens = infix.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (string token in tokens)
        {
            // Если токен - число или переменная, добавляем в выход
            if (float.TryParse(token, out _) || IsVariable(token))
            {
                output.Add(token);
            }
            // Если токен - оператор
            else if (IsOperator(token))
            {
                // Выталкиваем операторы с большим или равным приоритетом
                while (operators.Count > 0 && precedence.ContainsKey(operators.Peek()) &&
                       precedence[operators.Peek()] >= precedence[token])
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
            // Если токен - открывающая скобка
            else if (token == "(")
            {
                operators.Push(token);
            }
            // Если токен - закрывающая скобка
            else if (token == ")")
            {
                // Выталкиваем операторы до открывающей скобки
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }
                operators.Pop(); // Удаляем открывающую скобку "("
            }
        }

        // Выталкиваем оставшиеся операторы
        while (operators.Count > 0)
        {
            output.Add(operators.Pop());
        }

        return string.Join(' ', output);
    }

    /// <summary>
    /// Преобразует строку в обратной польской нотации в список токенов.
    /// </summary>
    /// <param name="rpn">Выражение в обратной польской нотации.</param>
    /// <returns>Список токенов.</returns>
    public static List<Token> TokenizeReversePolishNotation(string rpn)
    {
        List<Token> tokens = new();
        string[] parts = rpn.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            // Проверяем, является ли токен числом
            if (float.TryParse(part, out _))
            {
                tokens.Add(new Token(Token.TokenType.Number, part));
            }
            // Проверяем, является ли токен переменной
            else if (IsVariable(part))
            {
                tokens.Add(new Token(Token.TokenType.Variable, part));
            }
            // Проверяем, является ли токен оператором
            else if (IsOperator(part))
            {
                tokens.Add(new Token(Token.TokenType.Operator, part));
            }
            else
            {
                throw new Exception($"Unknown token: {part}");
            }
        }

        return tokens;
    }

    /// <summary>
    /// Проверяет, является ли строка переменной.
    /// </summary>
    /// <param name="str">Проверяемая строка.</param>
    /// <returns>True, если строка является переменной.</returns>
    private static bool IsVariable(string str)
    {
        // Пока поддерживается только переменная 'x'
        return str == "x";
    }

    /// <summary>
    /// Проверяет, является ли строка оператором или функцией.
    /// </summary>
    /// <param name="str">Проверяемая строка.</param>
    /// <returns>True, если строка является оператором или функцией.</returns>
    private static bool IsOperator(string str)
    {
        return str is "+" or "-" or "*" or "/" or "^" or "sin" or "cos" or "tan" or "log" or "exp";
    }
}