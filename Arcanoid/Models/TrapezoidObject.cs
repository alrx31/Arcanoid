using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;

namespace Arcanoid.Models
{
    public class TrapezoidObject : DisplayObject
    {
        public TrapezoidObject(Canvas canvas,int _maxX, int _maxY) : base(canvas,_maxX,_maxY)
        {
            var random = new Random();
            var randomWidth = random.Next(50, 70);  
            var randomHeight = random.Next(30, 70); 
            var topWidth = randomWidth * 0.6;        

            var color = GetRandomBrush();
            Shape = new Polygon()
            {
                Points = new Points
                {
                    new Point((randomWidth - topWidth) / 2, 0),    
                    new Point((randomWidth + topWidth) / 2, 0),    
                    new Point(randomWidth, randomHeight),          
                    new Point(0, randomHeight)                     
                },
                Fill = color,
                Width = randomWidth,
                Height = randomHeight,
                Stroke = Brushes.White,
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