using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;

namespace Arcanoid.Models
{
    public class TriangleShape : DisplayObject
    {
        public TriangleShape(Canvas canvas) : base(canvas)
        {
            var random = new Random();
            var randomSize = random.Next(30, 100); 

            var color = GetRandomBrush();
            Shape = new Polygon
            {
                Points = new Points
                {
                    new Point(0, randomSize),  
                    new Point(randomSize / 2, 0),   
                    new Point(randomSize, randomSize)
                },
                Fill = color,
                Width = randomSize,
                Height = randomSize
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