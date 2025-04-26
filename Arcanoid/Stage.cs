using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcanoid.Models;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace Arcanoid
{
    public class Stage
    {
        public Canvas GameCanvas { get; private set; }
        
        private readonly List<DisplayObject> _shapes;
        private bool _isRunning;
        private readonly double BASE_FPS = 16;

        public Stage()
        {
            GameCanvas = new Canvas
            {
                Background = Brushes.Black
            };
            _shapes = new List<DisplayObject>();
        }

        public void AddRandomShapes(int count, int _maxX, int _maxY)
        {
            var random = new Random();
            Console.WriteLine(_maxX + " - " + _maxY);
            
            for (int i = 0; i < count; i++)
            {
                var (R1, G1, B1) = GetRandomBrush();
                var (R2, G2, B2) = GetRandomBrush();

                int size = 200;//Random.Shared.Next(120, 150);
                int posX, posY;
                int iteration = 0;
                do
                {
                    (posX,posY) = (Random.Shared.Next(_maxX), Random.Shared.Next(_maxY));
                    iteration++;
                    if (iteration > 10_000)
                    {
                        Console.WriteLine("Less shapes or size");
                        break;
                    }
                }
                while (isPositionInValid(posX,posY,_maxX,_maxY,size));
                
                _shapes.Add(new CircleObject(
                    GameCanvas,
                    posX,
                    posY,
                    new List<int>{random.Next(50,100)},
                    new List<int>{size},
                    R1, G1, B1, R2, G2, B2
                    ));
                
                /*
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
                    new List<int>{random.Next(0,100),random.Next(0,100),random.Next(0,100),random.Next(0,100),random.Next(0,100),random.Next(0,100)},
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
                    ));*/
            }
        }

        private bool isPositionInValid(int posX, int posY, int _maxX, int _maxY, int size)
        {
            foreach (var shape in _shapes)
            {
                double distance = Math.Sqrt(Math.Pow(shape.X - posX, 2) + Math.Pow(shape.Y - posY, 2));

                if (distance < shape.Size[0] + shape.Size[0] || posX < 0 || posY < 0 || posX >= _maxX - 2*size || posY >= _maxY - 2*size)
                {
                    return true;
                }
            }
            
            return false;
        }

        public async void StartMovement(byte acc)
        {
            foreach (var shape in _shapes)
            {
                shape.StartMovement(acc);
            }

            _isRunning = true;

            while (_isRunning)
            {
                var (delta,shape1,shape2) = CalculateNextCollision();

                Console.WriteLine(delta);
                var time = (int)Math.Ceiling(delta);

                await Dispatcher.UIThread.InvokeAsync(async() =>
                {
                    await Task.Delay(time);
                    DrawNextFrame(delta, shape1,shape2);
                });
            }
        }

        private void DrawNextFrame(double time, DisplayObject shape1, DisplayObject shape2)
        {
            foreach (var shape in _shapes)
            {
                //if (time <= 0.1) time = 0.11;
                shape.Move(time);
            }
            
            if (time < BASE_FPS) HandleCollision(shape1, shape2);
        }

        private (double,DisplayObject,DisplayObject) CalculateNextCollision()
        {
            double minTime = BASE_FPS;
            DisplayObject sh1 = null;
            DisplayObject sh2 = null;
            
            for (int i = 0; i < _shapes.Count; i++)
            {
                for (int j = i + 1; j < _shapes.Count; j++)
                {
                    var shape1 = _shapes[i];
                    var shape2 = _shapes[j];
                    
                    var res = CalculateNextCollisionForTwoShapes(shape1, shape2);
                    if (res < minTime)
                    {
                        minTime = res;
                        sh1 = shape1;
                        sh2 = shape2;
                    }
                }
            }

            return (minTime,sh1,sh2);
        }

        private double CalculateNextCollisionForTwoShapes(DisplayObject shape1, DisplayObject shape2)
        {
            if (shape1 is CircleObject && shape2 is CircleObject)
            {
                double r1 = shape1.Size[0] / 2.0;
                double r2 = shape2.Size[0] / 2.0;
                double radiusSum = r1 + r2;

                double x1 = shape1.X + r1;
                double y1 = shape1.Y + r1;
                double x2 = shape2.X + r2;
                double y2 = shape2.Y + r2;

                double v1x = shape1.Speed * Math.Cos(shape1.AngleSpeed);
                double v1y = shape1.Speed * Math.Sin(shape1.AngleSpeed);
                double v2x = shape2.Speed * Math.Cos(shape2.AngleSpeed);
                double v2y = shape2.Speed * Math.Sin(shape2.AngleSpeed);

                double dx = x1 - x2;
                double dy = y1 - y2;
                double dvx = v1x - v2x;
                double dvy = v1y - v2y;

                double a = dvx * dvx + dvy * dvy;
                double b = 2 * (dx * dvx + dy * dvy);
                double c = dx * dx + dy * dy - radiusSum * radiusSum;

                if (c <= 0)
                    
                    return 0.01;

                if (a == 0)
                    return BASE_FPS;

                double discriminant = b * b - 4 * a * c;

                if (discriminant < 0)
                    return BASE_FPS;

                double sqrtDisc = Math.Sqrt(discriminant);
                double t1 = (-b - sqrtDisc) / (2 * a);
                double t2 = (-b + sqrtDisc) / (2 * a);

                if (t1 > 0)
                    return t1;
                if (t2 > 0)
                    return t2;

                return BASE_FPS;
            }

            return BASE_FPS;
        }
        
        public void StopMovement()
        {
            _isRunning = false;
        }

        public void ClearCanvas()
        {
            GameCanvas.Children.Clear();
            _shapes.Clear();
        }
        
        private void HandleCollision(DisplayObject shape1, DisplayObject shape2)
        {
            if (shape1 is CircleObject && shape2 is CircleObject)
            {
                double mass1 = 1;//shape1.Size[0];
                double mass2 = 1;//shape2.Size[0];
                
                double v1x = shape1.Speed * Math.Cos(shape1.AngleSpeed);
                double v1y = shape1.Speed * Math.Sin(shape1.AngleSpeed);

                double v2x = shape2.Speed * Math.Cos(shape2.AngleSpeed);
                double v2y = shape2.Speed * Math.Sin(shape2.AngleSpeed);

                double nx = shape2.X - shape1.X;
                double ny = shape2.Y - shape1.Y;
                double distance = Math.Sqrt(nx * nx + ny * ny);

                if (distance == 0) return;

                nx /= distance;
                ny /= distance;

                double relativeVelocityX = v2x - v1x;
                double relativeVelocityY = v2y - v1y;
                double dotProduct = relativeVelocityX * nx + relativeVelocityY * ny;

                if (dotProduct > 0)
                {
                    return;
                }

                double p1 = v1x * nx + v1y * ny;
                double p2 = v2x * nx + v2y * ny;

                double p1After = (p1 * (mass1 - mass2) + 2 * mass2 * p2) / (mass1 + mass2);
                double p2After = (p2 * (mass2 - mass1) + 2 * mass1 * p1) / (mass1 + mass2);

                v1x += (p1After - p1) * nx;
                v1y += (p1After - p1) * ny;

                v2x += (p2After - p2) * nx;
                v2y += (p2After - p2) * ny;

                //shape1.Speed = Math.Sqrt(v1x * v1x + v1y * v1y);
                shape1.AngleSpeed = Math.Atan2(v1y, v1x);

                //shape2.Speed = Math.Sqrt(v2x * v2x + v2y * v2y);
                shape2.AngleSpeed = Math.Atan2(v2y, v2x);

                double overlap = (double) shape1.Size[0] / 2 + (double) shape2.Size[0] /2 - distance;
                if(overlap < 0.01 && overlap > 0) overlap = 0.01;
                if (overlap > 0)
                {
                    double moveX = nx * (overlap / 2) + 0.1;
                    double moveY = ny * (overlap / 2) + 0.1;
                    
                    double centre1X = shape1.X + (double) shape1.Size[0] / 2;
                    double centre1Y = shape1.Y + (double) shape1.Size[0] / 2;
                    
                    double centre2X = shape2.X + (double) shape1.Size[0] / 2;
                    double centre2Y = shape2.Y + (double) shape1.Size[0] / 2;

                    if (centre1X < centre2X)
                    {
                        shape1.X -= moveX;
                        shape2.X += moveX;
                    }
                    else
                    {
                        shape1.X += moveX;
                        shape2.X -= moveX;
                    }

                    if (centre1Y < centre2Y)
                    {
                        shape1.Y -= moveY;
                        shape2.Y += moveY;
                    }
                    else
                    {
                        shape1.Y += moveY;
                        shape2.Y -= moveY;
                    }
                }
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
                    Size = shape.Size
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
                        shape = new CircleObject(GameCanvas,800,800,data.Size,data.Size,r1,g1,b1,r2,g2,b2)
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