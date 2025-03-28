using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Arcanoid.Models;

public abstract class DisplayObject
{
    public Shape Shape { get; set; }
    public Rectangle hitBoxShape;
    public Canvas Canvas { get; set; }
    public double X { get; set; }
    public double Y{ get; set; }
    public double Speed{ get; set; }
    public double AngleSpeed{ get; set; }
    public double Acceleration{ get; set; }
    public double AngleAcceleration{ get; set; }
    
    public byte r1,g1,b1,r2,g2,b2;
    public List<int> Size { get; set; }

    public DisplayObject(Canvas canvas, int _maxX, int _maxY)
    {
        this.Canvas = canvas;

        var rand = new Random();

        X = rand.Next(50, _maxX - 100);
        Y = rand.Next(50, _maxY - 100);
        Speed = rand.Next(1, 10);
        AngleSpeed = rand.NextDouble() * 2 * Math.PI;
        AngleAcceleration = rand.NextDouble() * 2 * Math.PI;
    }
    
    public void StartMovement(double acceleration)
    {
        Acceleration = acceleration;
    }

    public void Move()
    {
        double speedX = Speed * Math.Cos(AngleSpeed);
        double speedY = Speed * Math.Sin(AngleSpeed);

        double accelerationX = Acceleration * Math.Cos(AngleAcceleration);
        double accelerationY = Acceleration * Math.Sin(AngleAcceleration);

        X += speedX;
        Y += speedY;

        speedX += accelerationX;
        speedY += accelerationY;

        Speed = Math.Sqrt(speedX * speedX + speedY * speedY);

        AngleSpeed = Math.Atan2(speedY, speedX);

        if (X <= 0 || X >= Canvas.Bounds.Width - Shape.Width)
        {
            AngleSpeed = Math.PI - AngleSpeed; 
            X = Math.Max(0, Math.Min(X, Canvas.Bounds.Width - Shape.Width));
            //Speed *= 0.95;
        }
        if (Y <= 0 || Y >= Canvas.Bounds.Height - Shape.Height)
        {
            AngleSpeed = -AngleSpeed;
            Y = Math.Max(0, Math.Min(Y, Canvas.Bounds.Height - Shape.Height));
            //Speed *= 0.95;
        }

        Canvas.SetLeft(Shape, X);
        Canvas.SetTop(Shape, Y);
        
        //Canvas.SetLeft(hitBoxShape, X);
        //Canvas.SetTop(hitBoxShape, Y);
    }

    public abstract void Draw();
}