using Microsoft.VisualStudio.TestTools.UnitTesting;
using KoshechkinCalculatorLib;

namespace KoshechkinCalculatorTests;

[TestClass]
public sealed class CalculatorEngineTests
{
    private readonly CalculatorEngine _engine = new();

    [DataTestMethod]
    [DataRow("2+3", 5.0)]
    [DataRow("10-7", 3.0)]
    [DataRow("4*2.5", 10.0)]
    [DataRow("9/3", 3.0)]
    [DataRow("2^5", 32.0)]
    [DataRow("2+3*4", 14.0)]
    [DataRow("(2+3)*4", 20.0)]
    [DataRow("2^3^2", 512.0)]
    [DataRow("-2^2", -4.0)]
    [DataRow("1,5+2", 3.5)]
    public void Evaluate_ValidExpression_ReturnsExpected(string expression, double expected)
    {
        Assert.AreEqual(expected, _engine.Evaluate(expression), 0.000001);
    }

    [TestMethod]
    public void Evaluate_DivisionByZero_Throws()
    {
        Assert.ThrowsException<DivideByZeroException>(() => _engine.Evaluate("8/(3-3)"));
    }

    [TestMethod]
    public void Evaluate_MissingParenthesis_Throws()
    {
        Assert.ThrowsException<FormatException>(() => _engine.Evaluate("(2+4"));
    }

    [TestMethod]
    public void Evaluate_UnexpectedCharacter_Throws()
    {
        Assert.ThrowsException<FormatException>(() => _engine.Evaluate("2+q"));
    }
}

