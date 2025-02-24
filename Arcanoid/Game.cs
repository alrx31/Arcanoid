using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Arcanoid
{
    public class Game
    {
        private readonly Stage _stage;
        private readonly Window _mainWindow;
        private readonly GameMenu _menu;
        private Grid _mainGrid;
        private Canvas _menuCanvas;
        
        private bool _isFullScreen = false;
        private bool _isRunWithoutAcceleration = false;
        
        private bool _isRunWithAcceleration = false;
        private bool _isMenuOpen = false;

        public Game(Window window)
        {
            _mainWindow = window;
            _stage = new Stage();

            _menuCanvas = new Canvas
            {
                Background = Brushes.Transparent,
                IsHitTestVisible = false
            };

            _menu = new GameMenu(_menuCanvas, StartGame, SaveGame, LoadGame, Settings, Pause, Exit);

            _mainGrid = new Grid();
            _mainGrid.Children.Add(_stage.GameCanvas);
            _mainGrid.Children.Add(_menuCanvas);

            _mainWindow.Content = _mainGrid;
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
            else if (e.Key == Key.M) 
            {
                ToggleMenu();
            }
            else if (!_isMenuOpen) 
            {
                if (e.Key == Key.Space)
                {
                    if (_isRunWithAcceleration || _isRunWithoutAcceleration)
                    {
                        _stage.StopMovement();
                        _isRunWithAcceleration = false;
                        _isRunWithoutAcceleration = false;
                    }
                    else
                    {
                        _stage.StartMovement(0);
                        _isRunWithAcceleration = true;
                    }
                }
                else if (e.Key == Key.Z)
                {
                    if (_isRunWithAcceleration || _isRunWithoutAcceleration)
                    {
                        _stage.StopMovement();
                        _isRunWithAcceleration = false;
                        _isRunWithoutAcceleration = false;
                    }
                    else
                    {
                        _stage.StartMovement(new Random().NextDouble());
                        _isRunWithoutAcceleration = true;
                    }
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

        private void ToggleMenu()
        {
            if (_isMenuOpen)
            {
                _menuCanvas.IsHitTestVisible = false;
                _menuCanvas.Children.Clear(); 
                _stage.RemoveBlurEffect(); 
            }
            else
            {
                _menu.DrawMenu(); 
                _menuCanvas.IsHitTestVisible = true;
                _stage.ApplyBlurEffect();
            }
            _isMenuOpen = !_isMenuOpen;
        }

        private void StartGame()
        {
            _stage.ClearCanvas();
            Start();
            ToggleMenu();
        }

        private void SaveGame()
        {
            Console.WriteLine("Игра сохранена");
        }

        private void LoadGame()
        {
            Console.WriteLine("Игра загружена");
        }

        private void Settings()
        {
            Console.WriteLine("Открыты настройки");
        }

        private void Pause()
        {
            Console.WriteLine("Игра на паузе или выход");
            ToggleMenu();
        }

        private void Exit()
        {
            _mainWindow.Close();
        }
    }
}
