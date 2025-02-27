using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Arcanoid.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Arcanoid
{
    public class Game
    {
        private const string SaveFileName = "./../../../save.json";
        
        private readonly Stage _stage;
        private readonly Window _mainWindow;
        private readonly GameMenu _menu;
        private Grid _mainGrid;
        private Canvas _menuCanvas;
        
        private bool _isFullScreen = true;
        private bool _isRunWithoutAcceleration = false;
        
        private bool _isRunWithAcceleration = false;
        private bool _isMenuOpen = false;
        
        private int _shapeCount = 10;

        private int _maxX;
        private int _maxY;

        public Game(Window window)
        {
            _mainWindow = window;
            
            _mainWindow.WindowState = WindowState.FullScreen;
            _mainWindow.Width = 2318;
            _mainWindow.Height = 1449;

            var border = new Border
            {
                BorderBrush = Brushes.White, 
                BorderThickness = new Thickness(10),
                Padding = new Thickness(10),
            };
            
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
            
            border.Child = _mainGrid;
            
            _mainWindow.Content = border;
            _mainWindow.KeyDown += OnKeyDown;
            
            _maxX = (int)_mainWindow.Width;
            _maxY = (int)_mainWindow.Height;
        }

        public void Start()
        {
            _stage.AddRandomShapes(_shapeCount,(int)_mainWindow.Width,(int)_mainWindow.Height);
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


        public void SaveGame()
        {
            var shapesData = _stage.GetShapesData();
            var json = JsonSerializer.Serialize(shapesData, new JsonSerializerOptions { WriteIndented = true });
    
            File.WriteAllText(SaveFileName, json);
            Console.WriteLine("Игра сохранена");
        }

        public void LoadGame()
        {
            if (File.Exists(SaveFileName))
            {
                var json = File.ReadAllText(SaveFileName);
                var shapesData = JsonSerializer.Deserialize<List<ShapeData>>(json);
                _stage.LoadShapesData(shapesData);
                Console.WriteLine("Игра загружена");
            }
            else
            {
                Console.WriteLine("Файл сохранения не найден.");
            }
        }


        private void Settings()
        {
            var settingsWindow = new SettingsWindow(_shapeCount, OnShapeCountChanged);
            settingsWindow.ShowDialog(_mainWindow);
        }

        private void OnShapeCountChanged(int newCount)
        {
            _shapeCount = newCount;
            Console.WriteLine($"Количество фигур изменено на: {_shapeCount}");
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
