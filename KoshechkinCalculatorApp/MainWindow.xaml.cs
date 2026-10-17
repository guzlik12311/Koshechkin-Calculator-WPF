using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KoshechkinCalculatorLib;

namespace KoshechkinCalculatorApp;

public partial class MainWindow : Window
{
    private readonly CalculatorEngine _engine = new();

    public MainWindow() => InitializeComponent();

    private void KeyButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        var label = button.Tag?.ToString() ?? button.Content?.ToString() ?? string.Empty;
        if (label == "AC") { ExpressionBox.Text = "0"; ResultText.Text = "Введите выражение"; return; }
        if (label == "=") { Calculate(); return; }
        if (ExpressionBox.Text == "0" && (char.IsDigit(label[0]) || label == ",")) ExpressionBox.Clear();
        ExpressionBox.Text += label;
        ExpressionBox.CaretIndex = ExpressionBox.Text.Length;
        Preview();
    }

    private void Calculate()
    {
        try
        {
            var value = _engine.Evaluate(ExpressionBox.Text);
            ResultText.Text = "= " + value.ToString("G", CultureInfo.CurrentCulture);
        }
        catch (Exception ex) when (ex is FormatException or ArithmeticException)
        {
            ResultText.Text = ex.Message;
        }
    }

    private void Preview()
    {
        try { ResultText.Text = "≈ " + _engine.Evaluate(ExpressionBox.Text).ToString("G", CultureInfo.CurrentCulture); }
        catch { ResultText.Text = ""; }
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { Calculate(); e.Handled = true; }
        else if (e.Key == Key.Escape) { ExpressionBox.Text = "0"; ResultText.Text = "Введите выражение"; e.Handled = true; }
    }
}

