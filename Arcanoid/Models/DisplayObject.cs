using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Arcanoid.Models;

public abstract class DisplayObject
{
    protected Shape Shape;
    protected readonly Canvas Canvas;
    protected double X, Y;
    protected double Speed;
    protected double Angle; 
    protected double Acceleration;

    public DisplayObject(Canvas canvas)
    {
        this.Canvas = canvas;
        var rand = new Random();

        var canvasWidth = canvas.Bounds.Width > 0 ? canvas.Bounds.Width : 500;  
        var canvasHeight = canvas.Bounds.Height > 0 ? canvas.Bounds.Height : 500;

        X = rand.Next(0, (int)canvasWidth - 100);
        Y = rand.Next(0, (int)canvasHeight - 100);
        Speed = rand.Next(100, 200) / 100.0;
        Angle = rand.NextDouble() * 2 * Math.PI;
    }


    public void StartMovement(double acceleration)
    {
        Acceleration = acceleration;
    }

    public void Move()
    {
        X += Speed * Math.Cos(Angle);
        Y += Speed * Math.Sin(Angle);

        Speed += Acceleration;

        if (X < -Shape.Width/2 || X > Canvas.Bounds.Width - Shape.Width/2)
        {
            Angle = Math.PI - Angle; 
        }
        if (Y < -Shape.Height/2 || Y > Canvas.Bounds.Height - Shape.Height/2)
        {
            Angle = -Angle;
        }

        Canvas.SetLeft(Shape, X);
        Canvas.SetTop(Shape, Y);
    }

    protected Brush GetRandomBrush()
    {
        Random rand = new Random();
        byte r = (byte)rand.Next(256);
        byte g = (byte)rand.Next(256);
        byte b = (byte)rand.Next(256);

        return new SolidColorBrush(Color.FromRgb(r, g, b));
    }

    public abstract void Draw();
}