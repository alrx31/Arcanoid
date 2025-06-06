using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using Avalonia.Threading;

namespace Arcanoid.Models
{
    public class RectangleObject : DisplayObject
    {
        public RectangleObject(Canvas canvas,
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
            bool isSpetial,
            BaseBonusObject baseBonusObj
            ) : base(canvas,
            _maxX,
            _maxY,
            isSpetial,
            speed,
            baseBonusObj
            )
        {
            this.Size = size;
            this.r1 = R1;
            this.g1 = G1;
            this.b1 = B1;

            this.r2 = R2;
            this.g2 = G2;
            this.b2 = B2;
            
            var randomWidth = size[0];
            var randomHeight = size[1];

            Shape = new Rectangle
            {
                Width = randomWidth,
                Height = randomHeight,
                Fill = new SolidColorBrush(Color.FromRgb(R1,G1,B1)),
                Stroke = new SolidColorBrush(Color.FromRgb(R2,G2,B2)),
                StrokeThickness = 1
            };
            canvas.Children.Add(Shape);
            Draw();
        }

        public override void HandleCollision(DisplayObject other)
        {
            throw new NotImplementedException();
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
}