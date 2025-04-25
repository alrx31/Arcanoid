using System;
using Avalonia.Controls;
using Avalonia.Layout;

public class SettingsWindow : Window
{
    private readonly Action<int> _onShapeCountChanged;
    private NumericUpDown _shapeCountInput;

    public SettingsWindow(int currentShapeCount, Action<int> onShapeCountChanged)
    {
        _onShapeCountChanged = onShapeCountChanged;

        Title = "Настройки";
        Width = 300;
        Height = 150;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 10,
        };

        var label = new TextBlock
        {
            Text = "Количество фигур",
            FontSize = 16,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        _shapeCountInput = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 100,
            Value = currentShapeCount,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var applyButton = new Button
        {
            Content = "Применить",
            HorizontalAlignment = HorizontalAlignment.Center,
            Width = 100
        };
        applyButton.Click += ApplySettings;

        panel.Children.Add(label);
        panel.Children.Add(_shapeCountInput);
        panel.Children.Add(applyButton);

        Content = panel;
    }

    private void ApplySettings(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var newCount = (int)_shapeCountInput.Value;
        _onShapeCountChanged(newCount);
        Close();  
    }
}