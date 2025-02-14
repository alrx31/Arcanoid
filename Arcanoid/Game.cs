using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace Arcanoid;

public class Game
{
    private readonly Stage _stage;
    private readonly Window _mainWindow;
    private bool _isFullScreen = false;
    
    private bool _isRunWithoutAcceleration = false;
    private bool _isRunWithAcceleration = false;

    public Game(Window window)
    {
        _mainWindow = window;
        _stage = new Stage();
        _mainWindow.Content = _stage.Canvas;
        _mainWindow.KeyDown += OnKeyDown;
    }

    public void Start()
    {
        _stage.AddRandomShapes(10);
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.P) 
        {
            ToggleFullScreen();
        }
        if (e.Key == Key.Space)
        {
            if (_isRunWithAcceleration)
            {
                _stage.StopMovement();
                _isRunWithAcceleration = false;
                _isRunWithoutAcceleration = false;
            }
            else if (_isRunWithoutAcceleration)
            {
                _stage.StopMovement();
                _isRunWithAcceleration = false;
                _isRunWithoutAcceleration = false;
            }
            else
            {
                _stage.StartMovement(0);
                _isRunWithAcceleration = true;
                _isRunWithoutAcceleration = false;
            }
        }
        if (e.Key == Key.Z)
        {
            if (_isRunWithoutAcceleration)
            {
                _stage.StopMovement();
                _isRunWithAcceleration = false;
                _isRunWithoutAcceleration = false;
            }
            else if (_isRunWithAcceleration)
            {
                _stage.StopMovement();
                _isRunWithoutAcceleration = false;
                _isRunWithAcceleration = false;
            }
            else
            {
                _stage.StartMovement(new Random().NextDouble());
                _isRunWithoutAcceleration = true;
                _isRunWithAcceleration = false;
            }
        }
    }

    private void ToggleFullScreen()
    {
        if (_isFullScreen)
        {
            _mainWindow.WindowState = WindowState.Normal;
        }
        else
        {
            _mainWindow.WindowState = WindowState.FullScreen;
        }
        _isFullScreen = !_isFullScreen;
    }
}