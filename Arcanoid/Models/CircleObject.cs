using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Arcanoid.Models;

public class CircleObject : DisplayObject
{
    public CircleObject(Canvas canvas,
        int _maxX,
        int _maxY,
        List<int> size,
        byte R1,
        byte G1,
        byte B1,
        byte R2,
        byte G2,
        byte B2
        ) : base(canvas,
        _maxX,
        _maxY
        )
    {
        this.size = size;
        this.r1 = R1;
        this.g1 = G1;
        this.b1 = B1;

        this.r2 = R2;
        this.g2 = G2;
        this.b2 = B2;
        
        
        Shape = new Ellipse
        {
            Width = size[0],
            Height = size[0],
            Fill = new SolidColorBrush(Color.FromRgb(R1,G1,B1)),
            Stroke = new SolidColorBrush(Color.FromRgb(R2,G2,B2)),
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