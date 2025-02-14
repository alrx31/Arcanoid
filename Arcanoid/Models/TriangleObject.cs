using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Arcanoid.Models;

public class TriangleShape : DisplayObject
{
    public TriangleShape(Canvas canvas) : base(canvas)
    {
        var color = GetRandomBrush();
        Shape = new Polygon
        {
            Points = new Points
            {
                new Point(0, 50),  
                new Point(25, 0),   
                new Point(50, 50) 
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
