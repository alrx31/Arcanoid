using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;

namespace Arcanoid.Models
{
    public class RectangleObject : DisplayObject
    {
        public RectangleObject(Canvas canvas) : base(canvas)
        {
            var random = new Random();
            var randomWidth = random.Next(30, 50); 
            var randomHeight = random.Next(30, 50);

            Shape = new Rectangle
            {
                Width = randomWidth,
                Height = randomHeight,
                Fill = GetRandomBrush()
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