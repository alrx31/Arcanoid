using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Arcanoid.Models;

public abstract class DisplayObject
{
    public Shape Shape { get; set; }
    public Canvas Canvas { get; set; }
    public double X { get; set; } 
    public double Y{ get; set; }
    public double Speed{ get; set; }
    public double Angle{ get; set; }
    public double Acceleration{ get; set; }
    
    public byte r1,g1,b1,r2,g2,b2;
    public int size { get; set; }

    public DisplayObject(Canvas canvas, int _maxX, int _maxY)
    {
        this.Canvas = canvas;

        var rand = new Random();
        
        X = rand.Next(50, _maxX - 100);
        Y = rand.Next(50, _maxY - 100);
        Speed = rand.Next(1, 10);
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

    public abstract void Draw();
}