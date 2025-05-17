using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace Arcanoid.Models;

public class CircleObject : DisplayObject
{
    public bool isSpetialBall { get; set; }

    public CircleObject(Canvas canvas,
        int _maxX,
        int _maxY,
        List<int> size,
        double speed,
        byte R1,
        byte G1,
        byte B1,
        byte R2,
        byte G2,
        byte B2,
        bool isSpetialBall, BaseBonusObject baseBonusObj
        ) : base(canvas,
        _maxX,
        _maxY,
        isSpetialBall,
        speed,  
        baseBonusObj
        )
    {
        this.isSpetialBall = isSpetialBall;
        this.Size = size;
        this.r1 = R1;
        this.g1 = G1;
        this.b1 = B1;

        this.r2 = R2;
        this.g2 = G2;
        this.b2 = B2;

        Shape = new Ellipse()
        {
            Width = size[0],
            Height = size[0],
            Fill = new SolidColorBrush(Color.FromRgb(R1,G1,B1)),
            Stroke = new SolidColorBrush(Color.FromRgb(R2,G2,B2)),
            StrokeThickness = 1,
        };
        
        canvas.Children.Add(Shape);
        Draw();
    }

    public override void HandleCollision(DisplayObject other)
    {
        if (this is CircleObject shape1 && other is CircleObject shape2)
        {
            double mass1 = shape1.Size[0];
            double mass2 = shape2.Size[0];
                
            double v1x = shape1.Speed * Math.Cos(shape1.AngleSpeed);
            double v1y = shape1.Speed * Math.Sin(shape1.AngleSpeed);

            double v2x = shape2.Speed * Math.Cos(shape2.AngleSpeed);
            double v2y = shape2.Speed * Math.Sin(shape2.AngleSpeed);

            double nx = shape2.X - shape1.X;
            double ny = shape2.Y - shape1.Y;
            double distance = Math.Sqrt(nx * nx + ny * ny);

            if (distance == 0) return;

            nx /= distance;
            ny /= distance;

            double relativeVelocityX = v2x - v1x;
            double relativeVelocityY = v2y - v1y;
            double dotProduct = relativeVelocityX * nx + relativeVelocityY * ny;

            if (dotProduct > 0)
            {
                return;
            }

            double p1 = v1x * nx + v1y * ny;
            double p2 = v2x * nx + v2y * ny;

            double p1After = (p1 * (mass1 - mass2) + 2 * mass2 * p2) / (mass1 + mass2);
            double p2After = (p2 * (mass2 - mass1) + 2 * mass1 * p1) / (mass1 + mass2);

            v1x += (p1After - p1) * nx;
            v1y += (p1After - p1) * ny;

            v2x += (p2After - p2) * nx;
            v2y += (p2After - p2) * ny;

            //shape1.Speed = Math.Sqrt(v1x * v1x + v1y * v1y);
            shape1.AngleSpeed = Math.Atan2(v1y, v1x);

            //shape2.Speed = Math.Sqrt(v2x * v2x + v2y * v2y);
            shape2.AngleSpeed = Math.Atan2(v2y, v2x);
                
            double overlap = (double) shape1.Size[0] / 2 + (double) shape2.Size[0] /2 - distance;
            if(overlap is < 0.01 and > 0) overlap = 0.01;
            if (overlap > 0)
            {
                double moveX = nx * (overlap / 2) + 1;
                double moveY = ny * (overlap / 2) + 1;
                    
                shape1.X -= moveX;
                shape1.Y -= moveY;
                    
                shape2.X += moveX;
                shape2.Y += moveY;
            }
        }

        if (other is RectangleObject rect)
        {
            double circleCenterX = this.X + this.Size[0] / 2.0;
            double circleCenterY = this.Y + this.Size[0] / 2.0;

            double rectLeft = rect.X;
            double rectRight = rect.X + rect.Size[0];
            double rectTop = rect.Y;
            double rectBottom = rect.Y + rect.Size[1];

            this.AngleSpeed = -this.AngleSpeed;
            
            // Optional (bonus): Angle adjustment based on where the ball hits
            double hitPos = (circleCenterX - rectLeft) / rect.Size[0]; // 0 (left) → 1 (right)
            double angleVariation = (hitPos - 0.5) * Math.PI / 2; // Adjust max ±45°

            double newAngle = -Math.Abs(this.AngleSpeed) + angleVariation; 
            this.AngleSpeed = newAngle;
        }
    }

    public override void Draw()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            Canvas.SetLeft(Shape, X);
            Canvas.SetTop(Shape, Y);
        }, DispatcherPriority.Render);
    }
}