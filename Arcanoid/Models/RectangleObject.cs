using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Arcanoid.Models;

public class RectangleObject : DisplayObject
{
    public RectangleObject(Canvas canvas) : base(canvas)
    {
        Shape = new Rectangle
        {
            Width = 50,
            Height = 50,
            Fill = GetRandomBrush()
        };
        canvas.Children.Add(Shape);
        Draw();
    }

    public override void Draw()
    {
        Canvas.SetLeft(Shape, X);
        Canvas.SetTop(Shape, Y);
    }
}