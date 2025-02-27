using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;

namespace Arcanoid.Models
{
    public class RectangleObject : DisplayObject
    {
        public RectangleObject(Canvas canvas,int _maxX, int _maxY) : base(canvas,_maxX,_maxY)
        {
            var random = new Random();
            var randomWidth = random.Next(30, 150); 
            var randomHeight = random.Next(30, 150);

            Shape = new Rectangle
            {
                Width = randomWidth,
                Height = randomHeight,
                Fill = GetRandomBrush(),
                Stroke = GetRandomBrush(),
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