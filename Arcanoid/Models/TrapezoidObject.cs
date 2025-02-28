using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace Arcanoid.Models
{
    public class TrapezoidObject : DisplayObject
    {
        public TrapezoidObject(Canvas canvas,
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
            _maxY)
        {
            
            this.size = size;
            this.r1 = R1;
            this.g1 = G1;
            this.b1 = B1;

            this.r2 = R2;
            this.g2 = G2;
            this.b2 = B2;
            
            var randomWidth = size[0];
            var randomHeight = size[1]; 
            var topWidth = size[2];        

            Shape = new Polygon()
            {
                Points = new Points
                {
                    new Point((randomWidth - topWidth) / 2, 0),    
                    new Point((randomWidth + topWidth) / 2, 0),    
                    new Point(randomWidth, randomHeight),          
                    new Point(0, randomHeight)                     
                },
                Fill = new SolidColorBrush(Color.FromRgb(r1,g1,b1)),
                Width = randomWidth,
                Height = randomHeight,
                Stroke = new SolidColorBrush(Color.FromRgb(r2,g2,b2)),
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
}