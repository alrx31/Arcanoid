using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

public class GameMenu
{
    private readonly Canvas _menuCanvas;
    private readonly Action _startGame;
    private readonly Action _openSettings;
    private readonly Action _saveGame;
    private readonly Action _loadGame;
    private readonly Action _exitGame;
    private readonly Action _pauseGame;

    public GameMenu(Canvas menuCanvas, Action startGame, Action saveGame, Action loadGame, Action openSettings,Action _PauseGame, Action exitGame)
    {
        _menuCanvas = menuCanvas;
        _startGame = startGame;
        _saveGame = saveGame;
        _loadGame = loadGame;
        _openSettings = openSettings;
        _exitGame = exitGame;
        _pauseGame = _PauseGame;
    }

    public void DrawMenu()
    {
        _menuCanvas.Children.Clear();

        var grid = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Width = _menuCanvas.Bounds.Width,   
            Height = _menuCanvas.Bounds.Height, 
        };

        var menuPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center, 
            VerticalAlignment = VerticalAlignment.Center,     
            Spacing = 10
        };

        var fontSize = 32;
        var width = 300;
        var height = 80;
        var bg = new SolidColorBrush(Colors.Gray);

        var contButton = new Button
        {
            Content = "Продолжить",
            Width = width,
            Height = height,
            FontSize = fontSize,
            Background = bg
        };
        contButton.Click += (s,e) => _pauseGame();
        
        var startButton = new Button
        {
            Content = "Начать игру",
            Width = width,
            Height = height,
            FontSize = fontSize,
            Background = bg
        };
        startButton.Click += (s, e) => _startGame();

        var settingsButton = new Button
        {
            Content = "Настройки",
            Width = width,
            Height = height,
            FontSize = fontSize,
            Background = bg
        };
        settingsButton.Click += (s, e) => _openSettings();

        var saveButton = new Button
        {
            Content = "Сохранить игру",
            Width = width,
            Height = height,
            FontSize = fontSize,
            Background = bg
        };
        saveButton.Click += (s, e) => _saveGame();

        var loadButton = new Button
        {
            Content = "Загрузить игру",
            Width = width,
            Height = height,
            FontSize = fontSize,
            Background = bg
        };
        loadButton.Click += (s, e) => _loadGame();

        var exitButton = new Button
        {
            Content = "Выйти",
            Width = width,
            Height = height,
            FontSize = fontSize,
            Background = bg
        };
        exitButton.Click += (s, e) => _exitGame();

        menuPanel.Children.Add(contButton);
        menuPanel.Children.Add(startButton);
        menuPanel.Children.Add(settingsButton);
        menuPanel.Children.Add(saveButton);
        menuPanel.Children.Add(loadButton);
        menuPanel.Children.Add(exitButton);

        grid.Children.Add(menuPanel);

        _menuCanvas.Children.Add(grid);
    }
}
