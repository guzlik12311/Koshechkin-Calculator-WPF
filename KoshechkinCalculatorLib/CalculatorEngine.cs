using System.Globalization;

namespace KoshechkinCalculatorLib;

/// <summary>Вычисляет арифметическое выражение методом рекурсивного спуска.</summary>
public sealed class CalculatorEngine
{
    private string _text = string.Empty;
    private int _position;

    public double Evaluate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new FormatException("Введите арифметическое выражение.");

        _text = expression.Replace(',', '.');
        _position = 0;
        var result = ParseExpression();
        SkipSpaces();
        if (_position != _text.Length)
            throw new FormatException($"Неожиданный символ «{_text[_position]}».");
        if (!double.IsFinite(result))
            throw new ArithmeticException("Результат не является конечным числом.");
        return result;
    }

    private double ParseExpression()
    {
        var value = ParseTerm();
        while (true)
        {
            if (Take('+')) value += ParseTerm();
            else if (Take('-')) value -= ParseTerm();
            else return value;
        }
    }

    private double ParseTerm()
    {
        var value = ParseUnary();
        while (true)
        {
            if (Take('*')) value *= ParseUnary();
            else if (Take('/'))
            {
                var divisor = ParseUnary();
                if (divisor == 0) throw new DivideByZeroException("Деление на ноль невозможно.");
                value /= divisor;
            }
            else return value;
        }
    }

    private double ParseUnary()
    {
        if (Take('+')) return ParseUnary();
        if (Take('-')) return -ParseUnary();
        return ParsePower();
    }

    private double ParsePower()
    {
        var basis = ParsePrimary();
        if (Take('^')) basis = Math.Pow(basis, ParseUnary());
        return basis;
    }

    private double ParsePrimary()
    {
        SkipSpaces();
        if (Take('('))
        {
            var value = ParseExpression();
            if (!Take(')')) throw new FormatException("Не найдена закрывающая скобка.");
            return value;
        }

        SkipSpaces();
        var start = _position;
        var separatorSeen = false;
        while (_position < _text.Length)
        {
            var ch = _text[_position];
            if (char.IsDigit(ch)) { _position++; continue; }
            if (ch == '.' && !separatorSeen) { separatorSeen = true; _position++; continue; }
            break;
        }
        if (start == _position || _text[start] == '.')
            throw new FormatException("Ожидалось число или открывающая скобка.");
        if (!double.TryParse(_text[start.._position], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var number))
            throw new FormatException("Не удалось прочитать число.");
        return number;
    }

    private bool Take(char expected)
    {
        SkipSpaces();
        if (_position >= _text.Length || _text[_position] != expected) return false;
        _position++;
        return true;
    }

    private void SkipSpaces()
    {
        while (_position < _text.Length && char.IsWhiteSpace(_text[_position])) _position++;
    }
}

