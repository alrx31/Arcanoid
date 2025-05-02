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
        
        // 0 - platform, 1 - special ball
        private readonly List<DisplayObject> _shapes;
        
        private bool _isRunning;
        
        private readonly double BASE_FPS = 16; // in ms
        private readonly double EPSILON = 1; // expected calc error
        private readonly double PLATFORM_STEP_SIZE = 30;

        public Stage()
        {
            GameCanvas = new Canvas
            {
                Background = Brushes.Black
            };
            _shapes = new List<DisplayObject>();
        }

        #region AddObjects
        public void AddRandomShapes(int count, int _maxX, int _maxY)
        {
            Console.WriteLine(_maxX + " - " + _maxY);
            
            addPlatform(_maxX, _maxY);
            addSpetialBall(_maxX, _maxY);
            
            for (int i = 0; i < count; i++)
            {
                var (R1, G1, B1) = GetRandomBrush();
                var (R2, G2, B2) = GetRandomBrush();

                int size = Random.Shared.Next(100, 150);
                int posX, posY;
                int iteration = 0;
                do
                {
                    (posX,posY) = (Random.Shared.Next(_maxX), Random.Shared.Next(_maxY));
                    iteration++;
                    if (iteration > 100_000)
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
                    new List<int>{size},
                    R1, G1, B1, R2, G2, B2,
                    false
                    ));
            }
        }

        private void addSpetialBall(int _maxX, int _maxY)
        {
            _shapes.Add(new CircleObject(
                GameCanvas,
                _maxX/2 - 35,
                _maxY-200,
                new List<int>{70},
                255,255,255,0,0,0,
                true
            ));
        }

        private void addPlatform(int _maxX, int _maxY)
        {
            _shapes.Add(new Platform(
                GameCanvas,
                _maxX/2 - 200,
                _maxY-50,
                new List<int>{400,30},
                255,255,255,0,0,0,
                true
                ));
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
        #endregion
        
        public async void StartMovement(byte acc, double maxX, double maxY)
        {
            foreach (var shape in _shapes)
            {
                shape.StartMovement(acc);
            }

            _isRunning = true;

            while (_isRunning)
            {
                var (delta, shapes) = CalculateNextCollision(maxX,maxY);

                Console.WriteLine(delta);
                if(delta < EPSILON) delta = EPSILON;
                var time = (int)Math.Ceiling(delta);
                
                await Task.Delay(time);
                DrawNextFrame(delta, shapes);
            }
        }

        private void DrawNextFrame(
            double time,
            List<(DisplayObject, DisplayObject?)> shapes)
        {
            Dispatcher.UIThread.Invoke(()=>
            {
                foreach (var shape in _shapes)
                    MoveShape(shape, time);
            }, DispatcherPriority.Render);

            foreach (var (shape1,shape2) in shapes)
            {
                if (time < BASE_FPS) HandleCollision(shape1, shape2);
            }
            
        }

        #region Collision
        private (double, List<(DisplayObject, DisplayObject?)>) CalculateNextCollision(double maxX, double maxY)
        {
            var shapes = new List<(DisplayObject, DisplayObject)>();
            double minTime = BASE_FPS;
            
            for (int i = 0; i < _shapes.Count; i++)
            {
                var shape = _shapes[i];
                double vx = shape.Speed * Math.Cos(shape.AngleSpeed);
                double vy = shape.Speed * Math.Sin(shape.AngleSpeed);

                var distToLeft = shape.X;
                var distToRight = maxX - (shape.X + shape.Size[0]);
                
                var distToTop = shape.Y;
                var distToDown = maxY - (shape.Y + shape.Size[0]);

                vx = vx > 0 ? vx : -vx; 
                vy = vy > 0 ? vy : -vy; 
                
                var timeToLeft = distToLeft / vx;
                var timeToRight = distToRight / vx;
                var timeToTop = distToTop / vy;
                var timeToDown = distToDown / vy;

                var timeHorizontal = vx > 0 ? timeToRight : timeToLeft;
                var timeVertical = vy > 0 ? timeToTop : timeToDown;

                timeHorizontal = timeHorizontal == 0 ? 0.01 : timeHorizontal;
                timeVertical = timeVertical == 0 ? 0.01 : timeVertical;

                if (timeHorizontal < minTime)
                {
                    minTime = timeHorizontal;
                }else if (timeVertical < minTime)
                {
                    minTime = timeVertical;
                }else if (timeVertical < EPSILON || timeHorizontal < EPSILON)
                {
                    shapes.Add((shape, null));
                }
                
                for (int j = i + 1; j < _shapes.Count; j++)
                {
                    var shape1 = _shapes[i];
                    var shape2 = _shapes[j];
                    
                    var res = CalculateNextCollisionForTwoShapes(shape1, shape2);
                    if (res < minTime)
                    {
                        minTime = res;
                        shapes.Add((shape1,shape2));
                    }else if (res < EPSILON)
                    {
                        shapes.Add((shape1,shape2));
                    }
                }
            }

            return (minTime,shapes);
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
                
                double ov = Math.Sqrt(dx * dx + dy * dy) - (double)(shape1.Size[0] + shape2.Size[0])/ 2;

                if (ov < 0)
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

            if (shape1 is RectangleObject rect && shape2 is CircleObject circle)
            {
                var rectLeft = rect.X;
                var rectTop = rect.Y;
                var rectRight = rect.X + rect.Size[0];
                var rectBottom = rect.Y + rect.Size[1];

                var circleLeft = circle.X;
                var circleTop = circle.Y;
                var circleRight = circle.X + circle.Size[0];
                var circleBottom = circle.Y + circle.Size[0];

                //var rectSpeedX = rect.Speed * Math.Cos(rect.AngleSpeed);
                //var rectSpeedY = rect.Speed * Math.Sin(rect.AngleSpeed);

                var circleSpeedX = circle.Speed * Math.Cos(circle.AngleSpeed);
                var circleSpeedY = circle.Speed * Math.Sin(circle.AngleSpeed);

                var dvx = circleSpeedX;
                var dvy = circleSpeedY;

                double txEntry, txExit;
                if (dvx > 0)
                {
                    txEntry = (rectLeft - circleRight) / dvx;
                    txExit = (rectRight - circleLeft) / dvx;
                }
                else if (dvx < 0)
                {
                    txEntry = (rectRight - circleLeft) / dvx;
                    txExit = (rectLeft - circleRight) / dvx;
                }
                else
                {
                    txEntry = double.NegativeInfinity;
                    txExit = double.PositiveInfinity;
                }

                // Time to collide in Y
                double tyEntry, tyExit;
                if (dvy > 0)
                {
                    tyEntry = (rectTop - circleBottom) / dvy;
                    tyExit = (rectBottom - circleTop) / dvy;
                }
                else if (dvy < 0)
                {
                    tyEntry = (rectBottom - circleTop) / dvy;
                    tyExit = (rectTop - circleBottom) / dvy;
                }
                else
                {
                    tyEntry = double.NegativeInfinity;
                    tyExit = double.PositiveInfinity;
                }

                var entryTime = Math.Max(txEntry, tyEntry);
                var exitTime = Math.Min(txExit, tyExit);

                if (entryTime > exitTime || (txEntry < 0 && tyEntry < 0))
                {
                    Console.WriteLine("No collision.");
                }
                else
                {
                    return entryTime;
                }
            }


            return BASE_FPS;
        }
        
        private void HandleCollision(DisplayObject shape11, DisplayObject shape2)
        {
            if (shape11 is CircleObject shape1)
            {
                shape1.HandleCollision(shape2);
                if (shape1.isSpetialBall && shape2 is not null)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        _shapes.Remove(shape2);
                        GameCanvas.Children.Remove(shape2.Shape);
                    }, DispatcherPriority.Render);
                }
            }

            if (shape11 is RectangleObject shape1Rect)
            {
                shape2.HandleCollision(shape1Rect);
            }
        }
        
        #endregion
        
        public void StopMovement()
        {
            _isRunning = false;
        }

        public void ClearCanvas()
        {
            GameCanvas.Children.Clear();
            _shapes.Clear();
        }

        #region ShapeData
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
                        shape = new CircleObject(GameCanvas,800,800,data.Size,r1,g1,b1,r2,g2,b2,false)
                        {
                            X = data.X,
                            Y = data.Y,
                            Speed = data.Speed,
                            AngleSpeed = data.AngleSpeed,
                            Acceleration = data.Acceleration
                        };
                        break;
                    /*case "RectangleObject":
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
                        break;*/
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
        #endregion

        public void MoveShape(DisplayObject shape, double dt)
        {
            if (shape is Platform) return;
            double speedX = shape.Speed * Math.Cos(shape.AngleSpeed);
            double speedY = shape.Speed * Math.Sin(shape.AngleSpeed);

            double accelerationX = shape.Acceleration * Math.Cos(shape.AngleAcceleration);
            double accelerationY = shape.Acceleration * Math.Sin(shape.AngleAcceleration);

            var time = dt;
            shape.X += speedX*time;
            shape.Y += speedY*time;

            speedX += accelerationX;
            speedY += accelerationY;

            shape.Speed = Math.Sqrt(speedX * speedX + speedY * speedY);

            shape.AngleSpeed = Math.Atan2(speedY, speedX);

            if (shape.X <= 0 || shape.X >= shape.Canvas.Bounds.Width - shape.Shape.Width)
            {
                shape.AngleSpeed = Math.PI - shape.AngleSpeed; 
                shape.X = Math.Max(0, Math.Min(shape.X, shape.Canvas.Bounds.Width - shape.Shape.Width));
                //Speed *= 0.95;
            }
            if (shape.Y <= 0 || shape.Y >= shape.Canvas.Bounds.Height - shape.Shape.Height)
            {
                shape.AngleSpeed = -shape.AngleSpeed;
                shape.Y = Math.Max(0, Math.Min(shape.Y, shape.Canvas.Bounds.Height - shape.Shape.Height));
                //Speed *= 0.95;
            }

            if (shape.Y >= shape.Canvas.Bounds.Height - shape.Shape.Height && shape.isSpetial)
            {
                StopMovement();
            }
            
            shape.Draw();
        }

        public void MovePlatform(bool direction) // true - right
        {
            var platform = _shapes[0];
            var step = PLATFORM_STEP_SIZE;
            if (platform.X <= 0 && direction == false)
            {
                platform.X = 0;
                step = 0;
            }

            if (platform.X >= GameCanvas.Bounds.Width - platform.Shape.Width && direction == true)
            {
                platform.X = GameCanvas.Bounds.Width - platform.Shape.Width;
                step = 0;
            }
            platform.X += direction ? step : -step;
            platform.Draw();
        }
        
        public static (byte,byte,byte) GetRandomBrush()
        {
            Random rand = new Random();
            byte r = (byte)rand.Next(250);
            byte g = (byte)rand.Next(250);
            byte b = (byte)rand.Next(250);

            return (r,g,b);
        }
    }
}