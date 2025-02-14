using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Arcanoid
{
    public partial class MainWindow : Window
    {
        private readonly Game _game;

        public MainWindow()
        {
            InitializeComponent();
            _game = new Game(this);
            _game.Start();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}