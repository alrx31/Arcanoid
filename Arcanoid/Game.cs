using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Arcanoid.Models;
using Arcanoid.Special;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Arcanoid
{
    public class Game
    {
        private const string SaveFileName = "save.json";
        
        private readonly Stage _stage;
        private readonly Window _mainWindow;
        private readonly GameMenu _menu;

        private Statistics _statistics;
        
        private readonly Grid _mainGrid;
        private readonly Canvas _menuCanvas;
        
        private bool _isFullScreen = true;
        private bool _isRunWithoutAcceleration;
        
        private bool _isRunWithAcceleration;
        private bool _isMenuOpen;
        
        private int SHAPES_COUNT = 20;
        private readonly int START_LIVES_COUNT = 5; // after 0 game overs
        private readonly int START_DIFFICULTY_LEVEL = 1; // should be great than 0
        
        public Game(Window window)
        {
            _mainWindow = window;
            
            _mainWindow.WindowState = WindowState.FullScreen;
            _mainWindow.Width = 2298; //2298 - 1429
            _mainWindow.Height = 1429;

            var border = new Border
            {
                BorderBrush = Brushes.White, 
                BorderThickness = new Thickness(10),
                //Padding = new Thickness(10),
            };

            _statistics = new Statistics
            {
                DifficultyLevel = START_DIFFICULTY_LEVEL,
                LivesCount = START_LIVES_COUNT,
                Score = 0
            };
            
            _stage = new Stage(_statistics);
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
            _mainWindow.CanResize = true;
            
            // Events
            _mainWindow.KeyDown += OnKeyDown;
            // Resize
            _mainWindow.Resized += OnResize;
        }

        public void Start()
        {
            _stage.AddRandomShapes(SHAPES_COUNT,(int)_mainWindow.Width,(int)_mainWindow.Height);
        }

        private void OnResize(object sender, WindowResizedEventArgs e)
        {
            // Menu change position after resize
            ToggleMenu();
            ToggleMenu();

            _isRunWithAcceleration = false;
            _isRunWithoutAcceleration = true;
            _stage.StopMovement();

            _stage.StartMovement(0, _mainWindow.Width, _mainWindow.Height, _statistics);
            _isRunWithAcceleration = true;
            
            _stage.StopMovement();
            _isRunWithAcceleration = false;
            _isRunWithoutAcceleration = false;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                _stage.MovePlatform(true);
            }

            if (e.Key == Key.Left)
            {
                _stage.MovePlatform(false);
            }
            if (e.Key == Key.P)
            {
                ToggleFullScreen();
            }
            else if (e.Key == Key.M) 
            {
                _stage.StopMovement();
                _isRunWithAcceleration = false;
                _isRunWithoutAcceleration = false;
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
                        _stage.StartMovement(0, _mainWindow.Width, _mainWindow.Height, _statistics);
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
                        _stage.StartMovement(1, _mainWindow.Width, _mainWindow.Height, _statistics);
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
            
            ToggleMenu();
            ToggleMenu();
        }

        private void ToggleMenu()
        {
            if (_isMenuOpen)
            {
                _menuCanvas.IsHitTestVisible = false;
                _menuCanvas.Children.Clear(); 
            }
            else
            {
                _menu.DrawMenu(); 
                _menuCanvas.IsHitTestVisible = true;
            }
            _isMenuOpen = !_isMenuOpen;
        }

        private void StartGame()
        {
            _stage.ClearCanvas();
            Start();
            ToggleMenu();
        }

        public async void SaveGame()
        {
            var dialog = new SaveFileDialog()
            {
                Title = "Сохранение игры",
                DefaultExtension = "json",
                InitialFileName = SaveFileName,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "JSON Files", Extensions = { "json" } }
                }
            };
            var result = await dialog.ShowAsync(_mainWindow);
            if (!string.IsNullOrEmpty(result))
            {
                var shapesData = _stage.GetShapesData();
                var json = JsonSerializer.Serialize(shapesData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(result, json);
                Console.WriteLine("Игра сохранена: " + result);
            }
            else
            {
                Console.WriteLine("Сохранение отменено.");
            }
        }

        public async void LoadGame()
        {var dialog = new OpenFileDialog
            {
                Title = "Загрузка игры",
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "JSON Files", Extensions = { "json" } }
                }
            };

            var result = await dialog.ShowAsync(_mainWindow);
            if (result != null && result.Length > 0)
            {
                var filePath = result[0];
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var shapesData = JsonSerializer.Deserialize<List<ShapeData>>(json);
                    _stage.LoadShapesData(shapesData);
                    Console.WriteLine("Игра загружена: " + filePath);
                }
                else
                {
                    Console.WriteLine("Файл не найден.");
                }
            }
            else
            {
                Console.WriteLine("Загрузка отменена.");
            }
        }
        
        private void Settings()
        {
            var settingsWindow = new SettingsWindow(SHAPES_COUNT, OnShapeCountChanged);
            settingsWindow.ShowDialog(_mainWindow);
        }

        private void OnShapeCountChanged(int newCount)
        {
            SHAPES_COUNT = newCount;
            Console.WriteLine($"Количество фигур изменено на: {SHAPES_COUNT}");
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
