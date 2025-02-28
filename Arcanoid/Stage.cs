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
            var random = new Random();
            Console.WriteLine(_maxX + " - " + _maxY);
            
            for (int i = 0; i < count; i++)
            {
                var (R1, G1, B1) = GetRandomBrush();
                var (R2, G2, B2) = GetRandomBrush();
                
                _shapes.Add(new CircleObject(
                    GameCanvas,
                    _maxX,
                    _maxY,
                    new List<int>{random.Next(20,150)},
                    R1, G1, B1, R2, G2, B2
                    ));
                
                (R1, G1, B1) = GetRandomBrush();
                (R2, G2, B2) = GetRandomBrush();

                
                _shapes.Add(new RectangleObject(
                    GameCanvas,
                    _maxX,
                    _maxY,
                    new List<int>{random.Next(20,150),random.Next(20,150)},
                    R1, G1, B1, R2, G2, B2
                    ));
                
                (R1, G1, B1) = GetRandomBrush();
                (R2, G2, B2) = GetRandomBrush();

                _shapes.Add(new TriangleShape(
                    GameCanvas,
                    _maxX,
                    _maxY,
                    new List<int>{random.Next(20,70),random.Next(20,70),random.Next(20,70)},
                    R1, G1, B1, R2, G2, B2
                    ));
                
                (R1, G1, B1) = GetRandomBrush();
                (R2, G2, B2) = GetRandomBrush();

                _shapes.Add(new TrapezoidObject(
                    GameCanvas,
                    _maxX,
                    _maxY,
                    new List<int>{random.Next(20,70),random.Next(20,70),random.Next(20,70)},
                    R1, G1, B1, R2, G2, B2
                    ));
            }
        }

        public void StartMovement(byte acc)
        {
            foreach (var shape in _shapes)
            {
                shape.StartMovement(acc);
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
                    X = (int)shape.X,
                    Y = (int)shape.Y,
                    Speed = shape.Speed,
                    AngleSpeed = shape.AngleSpeed,
                    Acceleration = shape.Acceleration,
                    R1 = shape.r1,
                    G1 = shape.g1,
                    B1 = shape.b1,
                    R2 = shape.r2,
                    G2 = shape.g2,
                    B2 = shape.b2,
                    Size = shape.size
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

                byte r1, g1, b1;
                byte r2, g2, b2;

                r1 = data.R1;
                g1 = data.G1;
                b1 = data.B1;
                r2 = data.R2;
                g2 = data.G2;
                b2 = data.B2;

                switch (data.ShapeType)
                {
                    case "CircleObject":
                        shape = new CircleObject(GameCanvas,800,800,data.Size,r1,g1,b1,r2,g2,b2)
                        {
                            X = data.X,
                            Y = data.Y,
                            Speed = data.Speed,
                            AngleSpeed = data.AngleSpeed,
                            Acceleration = data.Acceleration
                        };
                        break;
                    case "RectangleObject":
                        shape = new RectangleObject(GameCanvas,800,800,data.Size, r1,g1,b1,r2,g2,b2)
                        {
                            X = data.X,
                            Y = data.Y,
                            Speed = data.Speed,
                            AngleSpeed = data.AngleSpeed,
                            Acceleration = data.Acceleration
                        };
                        break;
                    case "TriangleShape":
                        shape = new TriangleShape(GameCanvas,900,900,data.Size, r1,g1,b1,r2,g2,b2)
                        {
                            X = data.X,
                            Y = data.Y,
                            Speed = data.Speed,
                            AngleSpeed = data.AngleSpeed,
                            Acceleration = data.Acceleration
                        };
                        break;
                    case "TrapezoidObject":
                        shape = new TrapezoidObject(GameCanvas,900,900,data.Size, r1,g1,b1,r2,g2,b2)
                        {
                            X = data.X,
                            Y = data.Y,
                            Speed = data.Speed,
                            AngleSpeed = data.AngleSpeed,
                            Acceleration = data.Acceleration
                        };
                        break;
                }

                if (shape != null)
                {
                    shape.X = data.X;
                    shape.Y = data.Y;
                    shape.Speed = data.Speed;
                    shape.AngleSpeed = data.AngleSpeed;
                    shape.Acceleration = data.Acceleration;
                    shape.Draw();
                    _shapes.Add(shape);
                }
            }
        }

        
        public static (byte,byte,byte) GetRandomBrush()
        {
            Random rand = new Random();
            byte r = (byte)rand.Next(256);
            byte g = (byte)rand.Next(256);
            byte b = (byte)rand.Next(256);

            return (r,g,b);
        }
    }
}
