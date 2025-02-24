using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;

namespace Arcanoid.Models
{
    public class TrapezoidObject : DisplayObject
    {
        public TrapezoidObject(Canvas canvas) : base(canvas)
        {
            var random = new Random();
            var randomWidth = random.Next(50, 70);  // случайная ширина от 50 до 150
            var randomHeight = random.Next(30, 70); // случайная высота от 30 до 100
            var topWidth = randomWidth * 0.6;        // верхняя сторона трапеции меньше нижней

            var color = GetRandomBrush();
            Shape = new Polygon()
            {
                Points = new Points
                {
                    new Point((randomWidth - topWidth) / 2, 0),    // Левая вершина верхней стороны
                    new Point((randomWidth + topWidth) / 2, 0),    // Правая вершина верхней стороны
                    new Point(randomWidth, randomHeight),          // Правая нижняя вершина
                    new Point(0, randomHeight)                     // Левая нижняя вершина
                },
                Fill = color,
                Width = randomWidth,
                Height = randomHeight
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