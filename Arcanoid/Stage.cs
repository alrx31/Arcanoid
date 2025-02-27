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

        public void AddRandomShapes(int count, int _maxX, int _maxY)
        {
            Console.WriteLine(_maxX + " - " + _maxY);
            for (int i = 0; i < count; i++)
            {
                _shapes.Add(new CircleObject(GameCanvas,_maxX,_maxY));
                _shapes.Add(new RectangleObject(GameCanvas,_maxX,_maxY));
                _shapes.Add(new TriangleShape(GameCanvas,_maxX,_maxY));
                _shapes.Add(new TrapezoidObject(GameCanvas,_maxX,_maxY));
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
            _shapes.Clear();
        }

        public void ApplyBlurEffect()
        {
        }

        public void RemoveBlurEffect()
        {
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            foreach (var shape in _shapes)
            {
                shape.Move();
            }
        }
        
        public List<ShapeData> GetShapesData()
        {
            var shapesData = new List<ShapeData>();
            foreach (var shape in _shapes)
            {
                var data = new ShapeData
                {
                    ShapeType = shape.GetType().Name,
                    X = shape.X,
                    Y = shape.Y,
                    Speed = shape.Speed,
                    Angle = shape.Angle,
                    Acceleration = shape.Acceleration,
                };
                shapesData.Add(data);
            }
            return shapesData;
        }

        public void LoadShapesData(List<ShapeData> shapesData)
        {
            ClearCanvas();
            _shapes.Clear();

            foreach (var data in shapesData)
            {
                DisplayObject shape = null;

                switch (data.ShapeType)
                {
                    case "CircleObject":
                        shape = new CircleObject(GameCanvas,800,800)
                        {
                            X = data.X,
                            Y = data.Y,
                            Speed = data.Speed,
                            Angle = data.Angle,
                            Acceleration = data.Acceleration
                        };
                        break;
                    case "RectangleObject":
                        shape = new RectangleObject(GameCanvas,800,800)
                        {
                            X = data.X,
                            Y = data.Y,
                            Speed = data.Speed,
                            Angle = data.Angle,
                            Acceleration = data.Acceleration
                        };
                        break;
                    case "TriangleShape":
                        shape = new TriangleShape(GameCanvas,900,900)
                        {
                            X = data.X,
                            Y = data.Y,
                            Speed = data.Speed,
                            Angle = data.Angle,
                            Acceleration = data.Acceleration
                        };
                        break;
                    case "TrapezoidObject":
                        shape = new TrapezoidObject(GameCanvas,900,900)
                        {
                            X = data.X,
                            Y = data.Y,
                            Speed = data.Speed,
                            Angle = data.Angle,
                            Acceleration = data.Acceleration
                        };
                        break;
                }

                if (shape != null)
                {
                    shape.X = data.X;
                    shape.Y = data.Y;
                    shape.Speed = data.Speed;
                    shape.Angle = data.Angle;
                    shape.Acceleration = data.Acceleration;
                    shape.Draw();
                    _shapes.Add(shape);
                }
            }
        }

    }
}
