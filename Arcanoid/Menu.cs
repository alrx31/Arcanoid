using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Arcanoid
{
    public class GameMenu
    {
        private readonly Canvas _canvas;
        
        private Action _onStartGame;
        private Action _onSaveGame;
        private Action _onLoadGame;
        private Action _onSettings;
        private Action _onPauseExit;
        private Action _onExit;

        public GameMenu(Canvas canvas, Action onStartGame, Action onSaveGame, Action onLoadGame, Action onSettings, Action onPauseExit, Action onExit)
        {
            _canvas = canvas;
            _onStartGame = onStartGame;
            _onSaveGame = onSaveGame;
            _onLoadGame = onLoadGame;
            _onSettings = onSettings;
            _onPauseExit = onPauseExit;
            _onExit = onExit;
        }

        public void DrawMenu()
        {
            _canvas.Children.Clear();
            
            TextBlock menuTitle = new TextBlock
            {
                Text = "Меню игры",
                FontSize = 24,
                Foreground = Brushes.White,
                Margin = new Thickness(20)
            };
            _canvas.Children.Add(menuTitle);

            CreateMenuButton("Начать игру", 100, _onStartGame);
            CreateMenuButton("Сохранить игру", 150, _onSaveGame);
            CreateMenuButton("Загрузить игру", 200, _onLoadGame);
            CreateMenuButton("Настройки", 250, _onSettings);
            CreateMenuButton("Продолжить", 300, _onPauseExit);
            CreateMenuButton("Выход", 350, _onExit);
        }

        private void CreateMenuButton(string text, int topOffset, Action onClick)
        {
            Button button = new Button
            {
                Content = text,
                Width = 200,
                Height = 40,
                Margin = new Thickness(20, topOffset, 0, 0),
                Background = Brushes.Gray,
                Foreground = Brushes.White,
            };

            button.Click += (sender, args) => onClick();
            _canvas.Children.Add(button);
        }
    }
}
