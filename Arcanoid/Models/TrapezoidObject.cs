using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;

namespace Arcanoid.Models;

public class TrapezoidObject : DisplayObject
{
    public TrapezoidObject(Canvas canvas) : base(canvas)
    {
        var color = GetRandomBrush();
        Shape = new Polygon()
        {
            Points = new Points
            {
                new Point(15, 30),
                new Point(0, 0),
                new Point(35, 0),
                new Point(25, 30)
            },
            Fill = color,
        };
        Shape.Width = 50;
        Shape.Height = 50;
        
        canvas.Children.Add(Shape);
        Draw();
    }

    public override void Draw()
    {
        Canvas.SetLeft(Shape, X);
        Canvas.SetTop(Shape, Y);
    }
}