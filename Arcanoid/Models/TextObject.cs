using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace Arcanoid.Models;

public class TextObject : DisplayObject
{
    public TextBlock TextBlock { get; set; }
    private string _text { get; set; }
    
    public TextObject(Canvas canvas,
        int X,
        int Y,
        bool isSpetial,
        double speed,
        string text
        ) : base(canvas,
        X,
        Y,
        isSpetial,
        speed)
    {
        TextBlock = new TextBlock
        {
            Foreground = Brushes.White,
            FontSize = 48,
            TextWrapping = TextWrapping.Wrap,
        };
        Shape = new Rectangle()
        {
            Width = 50,
            Height = 50,
            Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
            StrokeThickness = 5
        };
        _text = text;
    }

    public override void HandleCollision(DisplayObject other)
    {
        throw new System.NotImplementedException();
    }

    public override void Draw()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            TextBlock.Text = _text;

            Canvas.SetLeft(TextBlock, X );
            Canvas.SetTop(TextBlock,  Y );
        }, DispatcherPriority.Render);
    }
}