using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Arcanoid.Models;

public class CircleObject : DisplayObject
{
    public CircleObject(Canvas canvas,int _maxX, int _maxY) : base(canvas,_maxX,_maxY)
    {
        var size = Random.Shared.Next(30, 170);
        Shape = new Ellipse
        {
            Width = size,
            Height = size,
            Fill = GetRandomBrush(),
            Stroke = GetRandomBrush(),
            StrokeThickness = 1
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