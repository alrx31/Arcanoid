using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace Arcanoid.Models;

public abstract class DisplayObject
{
    public Shape Shape { get; set; }
    public Canvas Canvas { get; set; }
    public double X { get; set; }
    public double Y{ get; set; }
    public double Speed{ get; set; }  // Dist/ms
    public double AngleSpeed{ get; set; }
    public double Acceleration{ get; set; }
    public double AngleAcceleration{ get; set; }
    public bool isSpetial { get; set; }
    public byte r1,g1,b1,r2,g2,b2;
    public List<int> Size { get; set; }
    public int ScoreValue { get; set; }
    public bool shouldSkip {get; set;}
    public BaseBonusObject BaseBonusObject { get; set; }

    public DisplayObject(Canvas canvas, int X, int Y, bool isSpetial,double speed, BaseBonusObject baseBonusObj)
    {
        this.isSpetial = isSpetial;
        this.Canvas = canvas;
        this.BaseBonusObject = baseBonusObj;

        var rand = new Random();

        this.X = X;
        this.Y = Y;
        Speed = speed;                            
        AngleSpeed = rand.NextDouble() * 2 * Math.PI;
        AngleAcceleration = rand.NextDouble() * 2 * Math.PI;
        ScoreValue = 100;
    }
    
    public void StartMovement(double acceleration)
    {
        Acceleration = acceleration / 100;
    }
    
    public abstract void HandleCollision(DisplayObject other);
    public abstract void Draw();
}