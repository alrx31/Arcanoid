using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Arcanoid.Models;

public class CircleObject : DisplayObject
{
    public CircleObject(Canvas canvas) : base(canvas)
    {
        var size = Random.Shared.Next(30, 70);
        Shape = new Ellipse
        {
            Width = size,
            Height = size,
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