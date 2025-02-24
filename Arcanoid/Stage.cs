using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using Arcanoid.Models;
using Avalonia.Threading;

namespace Arcanoid
{
    public class Stage
    {
        public Canvas GameCanvas { get; private set; }
        private readonly List<DisplayObject> _shapes;
        private readonly DispatcherTimer _timer;

        public Stage()
        {
            GameCanvas = new Canvas
            {
                Background = Brushes.Black
            };
            _shapes = new List<DisplayObject>();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _timer.Tick += OnTimerTick;
        }

        public void AddRandomShapes(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _shapes.Add(new CircleObject(GameCanvas));
                _shapes.Add(new RectangleObject(GameCanvas));
                _shapes.Add(new TriangleShape(GameCanvas));
                _shapes.Add(new TrapezoidObject(GameCanvas));
            }
        }

        public void StartMovement(double acceleration)
        {
            foreach (var shape in _shapes)
            {
                shape.StartMovement(acceleration);
            }
            _timer.Start();
        }

        public void StopMovement()
        {
            _timer.Stop();
        }

        public void ClearCanvas()
        {
            GameCanvas.Children.Clear();
        }

        public void ApplyBlurEffect()
        {
            GameCanvas.Effect = new BlurEffect
            {
                Radius = 10
            };
        }

        public void RemoveBlurEffect()
        {
            GameCanvas.Effect = null;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            foreach (var shape in _shapes)
            {
                shape.Move();
            }
        }
    }
}
