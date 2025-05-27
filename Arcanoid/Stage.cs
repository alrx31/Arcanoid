using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arcanoid.Models;
using Arcanoid.Models.Bonuses;
using Arcanoid.Special;
using Avalonia.Controls;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Media;
using Avalonia.Threading;

namespace Arcanoid
{
    public class Stage
    {
        public Canvas GameCanvas { get; private set; }
        private Statistics baseStatistics { get; set; }
        public Statistics Statistics { get; private set; }

        private TextBlock _statistics { get; set; }
        private TextBlock _fullScreenMessage { get; set; }

        // 0 - platform, 1 - special ball
        private readonly List<DisplayObject> _shapes;

        private bool _isRunning;

        private int SHAPES_COUNT;

        private readonly int SCORE_FOR_LEVEL = 1000;
        private readonly int MIN_LIVES = 0;                // when stop
        private readonly double BASE_FPS = 16;             // in ms
        private readonly double EPSILON = 1;               // expected calc error
        private readonly double PLATFORM_STEP_SIZE = 30;
        
        // Bonuses
        private readonly double BONUS_CHANCE_VALUE = .7;
        private readonly int BONUSES_COUNT = 14;
        
        private readonly DispatcherTimer _timer;

        public Stage(Statistics statistics)
        {
            GameCanvas = new Canvas
            {
                Background = Brushes.Black
            };
            _shapes = new List<DisplayObject>();
            this.Statistics = statistics;
            this.baseStatistics = new Statistics
            {
                DifficultyLevel = statistics.DifficultyLevel,
                LivesCount = statistics.LivesCount,
                Score = statistics.Score,
            };
            
            InitMessageBlock();
            DrawInitStatistics();
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(5)
            };
            _timer.Tick += OnTimerTick;
        }
        
        private void OnTimerTick(object sender, EventArgs e)
        {
            for (var i = 0; i < _shapes.Count; i++)
            {
                var shape = _shapes[i];
                CheckCollision(shape,i);
                MoveShape(shape, 5);
            }
        }
        
        private void CheckCollision(DisplayObject shape,int idx)
        {
            for (int i = idx+1; i < _shapes.Count; i++)
            {
                if (IsColliding(_shapes[i], shape))
                {
                    //wq    StopMovement();
                    HandleCollision(shape,_shapes[i]);
                }
            }
        }
        
        private bool IsColliding(DisplayObject shape1, DisplayObject shape2)
        {
            // --- Круг – Круг ---
            if (shape1 is CircleObject && shape2 is CircleObject)
            {
                double dx = (shape1.X + shape1.Size[0] / 2.0) - (shape2.X + shape2.Size[0] / 2.0);
                double dy = (shape1.Y + shape1.Size[0] / 2.0) - (shape2.Y + shape2.Size[0] / 2.0);
                double distance = Math.Sqrt(dx * dx + dy * dy);

                double radius1 = shape1.Size[0] / 2.0;
                double radius2 = shape2.Size[0] / 2.0;

                return distance <= radius1 + radius2;
            }

            // --- Круг – Прямоугольник ---
            if ((shape1 is CircleObject && shape2 is RectangleObject) ||
                (shape1 is RectangleObject && shape2 is CircleObject))
            {
                CircleObject circle = shape1 is CircleObject ? (CircleObject)shape1 : (CircleObject)shape2;
                RectangleObject rect = shape1 is RectangleObject ? (RectangleObject)shape1 : (RectangleObject)shape2;

                double circleX = circle.X + circle.Size[0] / 2.0;
                double circleY = circle.Y + circle.Size[0] / 2.0;
                double radius = circle.Size[0] / 2.0;

                double rectLeft = rect.X;
                double rectTop = rect.Y;
                double rectRight = rect.X + rect.Size[0];
                double rectBottom = rect.Y + rect.Size[1];

                double closestX = Math.Max(rectLeft, Math.Min(circleX, rectRight));
                double closestY = Math.Max(rectTop, Math.Min(circleY, rectBottom));

                double dx = circleX - closestX;
                double dy = circleY - closestY;

                return (dx * dx + dy * dy) <= (radius * radius);
            }

            // --- Прямоугольник – Бонус (BaseBonusObject) ---
            if (shape2 is RectangleObject rect1 && shape1 is BaseBonusObject rect2)
            {
                double r1Left = rect1.X;
                double r1Top = rect1.Y;
                double r1Right = rect1.X + rect1.Size[0];
                double r1Bottom = rect1.Y + rect1.Size[1];

                double r2Left = rect2.X;
                double r2Top = rect2.Y;
                double r2Right = rect2.X + rect2.Size[0];
                double r2Bottom = rect2.Y + rect2.Size[1];

                bool isOverlapping = !(r1Right < r2Left || r1Left > r2Right || r1Bottom < r2Top || r1Top > r2Bottom);
                return isOverlapping;
            }

            return false;
        }



        #region AddObjects

        public void AddRandomShapes(int count, int _maxX, int _maxY)
        {
            this.SHAPES_COUNT = count;
            
            InitMessageBlock();
            DrawInitStatistics();
            DrawStatistics(Statistics);

            Console.WriteLine(_maxX + " - " + _maxY);

            addPlatform(_maxX, _maxY);
            addSpetialBall(_maxX, _maxY);

            AddNotSpecShapes(count, _maxX, _maxY);
        }

        private void AddNotSpecShapes(int count, int _maxX, int _maxY)
        {
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var (R1, G1, B1) = GetRandomBrush();
                    var (R2, G2, B2) = GetRandomBrush();

                    int size = Random.Shared.Next(100, 150);
                    int posX, posY;
                    int iteration = 0;
    
                    double Speed = (double)Random.Shared.Next(1, 3+Statistics.DifficultyLevel / 2 )/4; 
                
                    do
                    {
                        (posX, posY) = (Random.Shared.Next(_maxX), Random.Shared.Next(_maxY));
                        iteration++;
                        if (iteration > 100_000)
                        {
                            Console.WriteLine("Less shapes or size");
                            break;
                        }
                    } while (isPositionInValid(posX, posY, _maxX, _maxY, size));

                    BaseBonusObject baseBonus = addBonus();
                
                    _shapes.Add(new CircleObject(
                        GameCanvas,
                        posX,
                        posY,
                        new List<int> { size },
                        Speed,
                        R1, G1, B1, R2, G2, B2,
                        false,
                        baseBonus
                    ));
                }
            }
            else
            {
                int j = 0;
                foreach (var s in _shapes.ToList())
                {
                    if (j == count) break;
                    if (!s.isSpetial && s is CircleObject)
                    {
                        RemoveNotSpecShape(s);
                        j--;
                    }
                }
            }
        }

        private void addSpetialBall(int _maxX, int _maxY)
        {
            
            _shapes.Add(new CircleObject(
                GameCanvas,
                _maxX / 2 - 35,
                _maxY - 200,
                new List<int> { 70 },
                (double) Random.Shared.Next(2, 5 + (Statistics.DifficultyLevel) / 2) / 4,
                255, 255, 255, 0, 0, 0,
                true,
                null
            ));
        }

        private void addPlatform(int _maxX, int _maxY)
        {
            _shapes.Add(new Platform(
                GameCanvas,
                _maxX / 2 - 200,
                _maxY - 40,
                new List<int> { 400, 30 },
                255, 255, 255, 0, 0, 0,
                true
            ));
        }

        private bool isPositionInValid(int posX, int posY, int _maxX, int _maxY, int size)
        {
            foreach (var shape in _shapes)
            {
                double distance = Math.Sqrt(Math.Pow(shape.X - posX, 2) + Math.Pow(shape.Y - posY, 2));

                if (distance < shape.Size[0] + shape.Size[0] || posX < 0 || posY < 0 || posX >= _maxX - 2 * size ||
                    posY >= _maxY - 2 * size)
                {
                    return true;
                }
            }

            return false;
        }

        private BaseBonusObject addBonus()
        {
            if (Random.Shared.NextDouble() < BONUS_CHANCE_VALUE)
            {
                var res = Random.Shared.Next(1,BONUSES_COUNT);

                return res switch
                {
                    1 => new ChangePlatformSizeBaseBonus(
                            GameCanvas,
                            0,
                            0,
                            0.8,
                            50,
                            100,
                            100,
                            100,
                            ApplyChangePlatformSize,
                            RemoveChangePlatformSize
                        )
                        {
                            AngleSpeed = double.Pi/2
                        },
                    2 => new ChangePlatformSizeBaseBonus(
                            GameCanvas,
                            0,
                            0,
                            0.8,
                            100,
                            100,
                            100,
                            100,
                            ApplyChangePlatformSize,
                            RemoveChangePlatformSize
                        )
                        {
                            AngleSpeed = double.Pi/2
                        },
                    3 => new ChangePlatformSizeBaseBonus(
                            GameCanvas,
                            0,
                            0,
                            0.8,
                            -50,
                            255,
                            0,
                            0,
                            ApplyChangePlatformSize,
                            RemoveChangePlatformSize
                        )
                        {
                            AngleSpeed = double.Pi/2
                        },
                    4 => new ChangePlatformSizeBaseBonus(
                            GameCanvas,
                            0,
                            0,
                            0.8,
                            -100,
                            255,
                            0,
                            0,
                            ApplyChangePlatformSize,
                            RemoveChangePlatformSize
                        )
                        {
                            AngleSpeed = double.Pi/2
                        },
                    5 => new ChangeSpecialBallSpeedBonus(
                        GameCanvas,
                        0,
                        0,
                        0.8,
                        2,
                        255,
                        0,
                        0,
                        ApplyChangeSpecialBallSpeed,
                        RemoveChangeSpecialBallSpeed
                    )
                    {
                        AngleSpeed = double.Pi/2
                    },
                    6 => new ChangeSpecialBallSpeedBonus(
                        GameCanvas,
                        0,
                        0,
                        0.8,
                        0.25,
                        100,
                        100,
                        100,
                        ApplyChangeSpecialBallSpeed,
                        RemoveChangeSpecialBallSpeed
                    )
                    {
                        AngleSpeed = double.Pi/2
                    },
                    7 => new ChangeNotSpecialBallSpeedBonus(
                        GameCanvas,
                        0,
                        0,
                        0.8,
                        0.25,
                        100,
                        100,
                        100,
                        ApplyChangeNotSpecBallSpeed,
                        RemoveChangeNotSpecBallSpeed
                    )
                    {
                        AngleSpeed = double.Pi/2
                    },
                    8 => new ChangeNotSpecialBallSpeedBonus(
                        GameCanvas,
                        0,
                        0,
                        0.8,
                        0.125,
                        100,
                        100,
                        100,
                        ApplyChangeNotSpecBallSpeed,
                        RemoveChangeNotSpecBallSpeed
                    )
                    {
                        AngleSpeed = double.Pi/2
                    },
                    9 => new ChangeNotSpecialBallSpeedBonus(
                        GameCanvas,
                        0,
                        0,
                        0.8,
                        2,
                        255,
                        0,
                        0,
                        ApplyChangeNotSpecBallSpeed,
                        RemoveChangeNotSpecBallSpeed
                    )
                    {
                        AngleSpeed = double.Pi/2
                    },
                    10 => new ChangeNotSpecialBallSpeedBonus(
                        GameCanvas,
                        0,
                        0,
                        0.8,
                        4,
                        255,
                        0,
                        0,
                        ApplyChangeNotSpecBallSpeed,
                        RemoveChangeNotSpecBallSpeed
                    )
                    {
                        AngleSpeed = double.Pi/2
                    },
                    11 => new AddNotSpecBalls(
                        GameCanvas,
                        0,
                        0,
                        0.8,
                        -2,
                        100,
                        100,
                        100,
                        ApplyAddNotSpecBalls,
                        RemoveAddNotSpecBalls
                    )
                    {
                        AngleSpeed = double.Pi/2
                    },
                    12 => new AddNotSpecBalls(
                        GameCanvas,
                        0,
                        0,
                        0.8,
                        -4,
                        100,
                        100,
                        100,
                        ApplyAddNotSpecBalls,
                        RemoveAddNotSpecBalls
                    )
                    {
                        AngleSpeed = double.Pi/2
                    },
                    13 => new AddNotSpecBalls(
                        GameCanvas,
                        0,
                        0,
                        0.8,
                        2,
                        255,
                        0,
                        0,
                        ApplyAddNotSpecBalls,
                        RemoveAddNotSpecBalls
                    )
                    {
                        AngleSpeed = double.Pi/2
                    },
                    14 => new AddNotSpecBalls(
                        GameCanvas,
                        0,
                        0,
                        0.8,
                        4,
                        255,
                        0,
                        0,
                        ApplyAddNotSpecBalls,
                        RemoveAddNotSpecBalls
                    )
                    {
                        AngleSpeed = double.Pi/2
                    }
                };
            }

            return null;
        }
        
        #endregion

        public async void StartMovement(byte acc, double maxX, double maxY, Statistics statistics)
        {
            /*DrawMessage("");
            foreach (var shape in _shapes)
            {
                shape.StartMovement(acc);
            }

            _isRunning = true;

            while (_isRunning)
            {
                var (delta, shapes) = CalculateNextCollision(maxX, maxY);

                //Console.WriteLine(delta);
                if (delta < EPSILON) delta = EPSILON;
                var time = (int)Math.Ceiling(delta);

                await Task.Delay(time);
                DrawNextFrame(delta, shapes);
            }*/
                foreach (var shape in _shapes)
                {
                    shape.StartMovement(acc);
                }
                _timer.Start();
        }

        private void DrawNextFrame(
            double time,
            List<(DisplayObject, DisplayObject?)> shapes)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                var shapesCopy = new List<DisplayObject>(_shapes);
                foreach (var shape in shapesCopy)
                {
                    if (shape != null)
                        MoveShape(shape, time);
                }
            }, DispatcherPriority.Render);

            foreach (var (shape1, shape2) in shapes)
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
                if(shape.shouldSkip) continue;
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
                }
                else if (timeVertical < minTime)
                {
                    minTime = timeVertical;
                }
                else if (timeVertical < EPSILON || timeHorizontal < EPSILON)
                {
                    shapes.Add((shape, null));
                }

                for (int j = i + 1; j < _shapes.Count; j++)
                {
                    var shape1 = _shapes[i];
                    var shape2 = _shapes[j];
                    if (!(shape1 is Platform && shape2 is BaseBonusObject))
                    {
                        if(shape2.shouldSkip) continue;
                    }
                    
                    var res = CalculateNextCollisionForTwoShapes(shape1, shape2);
                    if (res < minTime)
                    {
                        minTime = res;
                        shapes.Add((shape1, shape2));
                    }
                    else if (res < EPSILON)
                    {
                        shapes.Add((shape1, shape2));
                    }
                }
            }

            return (minTime, shapes);
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

                double ov = Math.Sqrt(dx * dx + dy * dy) - (double)(shape1.Size[0] + shape2.Size[0]) / 2;

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

                if (!(entryTime > exitTime) && (!(txEntry < 0) || !(tyEntry < 0)))
                {
                    return entryTime;
                }
            }

            if (shape1 is RectangleObject rect1 && shape2 is BaseBonusObject rect2)
            {
                double r1Left = rect1.X;
                double r1Top = rect1.Y;
                double r1Right = rect1.X + rect1.Size[0];
                double r1Bottom = rect1.Y + rect1.Size[1];

                double r2Left = rect2.X;
                double r2Top = rect2.Y;
                double r2Right = rect2.X + rect2.Size[0];
                double r2Bottom = rect2.Y + rect2.Size[1];

                double r1SpeedX = rect1.Speed * Math.Cos(rect1.AngleSpeed);
                double r1SpeedY = rect1.Speed * Math.Sin(rect1.AngleSpeed);
                double r2SpeedX = rect2.Speed * Math.Cos(rect2.AngleSpeed);
                double r2SpeedY = rect2.Speed * Math.Sin(rect2.AngleSpeed);

                double dvx = r1SpeedX - r2SpeedX;
                double dvy = r1SpeedY - r2SpeedY;

                double txEntry, txExit;
                if (dvx > 0)
                {
                    txEntry = (r2Left - r1Right) / dvx;
                    txExit = (r2Right - r1Left) / dvx;
                }
                else if (dvx < 0)
                {
                    txEntry = (r2Right - r1Left) / dvx;
                    txExit = (r2Left - r1Right) / dvx;
                }
                else
                {
                    txEntry = double.NegativeInfinity;
                    txExit = double.PositiveInfinity;
                }

                double tyEntry, tyExit;
                if (dvy > 0)
                {
                    tyEntry = (r2Top - r1Bottom) / dvy;
                    tyExit = (r2Bottom - r1Top) / dvy;
                }
                else if (dvy < 0)
                {
                    tyEntry = (r2Bottom - r1Top) / dvy;
                    tyExit = (r2Top - r1Bottom) / dvy;
                }
                else
                {
                    tyEntry = double.NegativeInfinity;
                    tyExit = double.PositiveInfinity;
                }

                double entryTime = Math.Max(txEntry, tyEntry);
                double exitTime = Math.Min(txExit, tyExit);

                if (entryTime < exitTime && (txEntry >= 0 || tyEntry >= 0) && entryTime >= 0)
                {
                    return entryTime;
                }

                bool isOverlapping = !(r1Right < r2Left || r1Left > r2Right || r1Bottom < r2Top || r1Top > r2Bottom);
                if (isOverlapping)
                {
                    return 0.01;
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
                    RemoveNotSpecShape(shape2);
                }
            }
            
            if (shape11 is RectangleObject shape1Rect)
            {
                shape2.HandleCollision(shape1Rect);

                if (shape2 is BaseBonusObject shape22)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        _shapes.Remove(shape2);
                        GameCanvas.Children.Remove(shape2.Shape);
                    }, DispatcherPriority.Render);
                }
            }
        }

        private void RemoveNotSpecShape(DisplayObject shape)
        {
            Dispatcher.UIThread.Invoke(async () =>
            {
                Statistics.Score += shape.ScoreValue;
                DrawStatistics(Statistics);
                var bonus = shape.BaseBonusObject;

                if (bonus is not null)
                {
                    bonus.X = shape.X;
                    bonus.Y = shape.Y;
                    AddBonusObject(bonus);
                }        
                
                _shapes.Remove(shape);
                GameCanvas.Children.Remove(shape.Shape);
                        
                AddTextBox(shape.ScoreValue.ToString(), shape.X - (double)shape.Size[0] / 2 , shape.Y - (double)shape.Size[0] / 2);
                        
                if (_shapes.Count(x => !x.isSpetial) == 0) // Platform + spec ball + all text boxes + all bonuses
                {
                    Statistics.Score += Statistics.DifficultyLevel++ * SCORE_FOR_LEVEL;
                    StopMovement();
                    await Task.Delay(2000);
                    StartNewGame();
                }
                        
            }, DispatcherPriority.Render);
        }

        #endregion

        public void StopMovement()
        {
            _timer.Stop();
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
                        shape = new CircleObject(GameCanvas, 800, 800, data.Size,data.Speed, r1, g1, b1, r2, g2, b2, false, null)
                        {
                            X = data.X,
                            Y = data.Y,
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

        public async void MoveShape(DisplayObject shape, double dt)
        {
            if (shape is Platform) return;
            double speedX = shape.Speed * Math.Cos(shape.AngleSpeed);
            double speedY = shape.Speed * Math.Sin(shape.AngleSpeed);

            double accelerationX = shape.Acceleration * Math.Cos(shape.AngleAcceleration);
            double accelerationY = shape.Acceleration * Math.Sin(shape.AngleAcceleration);

            var time = dt;
            shape.X += speedX * time;
            shape.Y += speedY * time;

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
                if (shape is CircleObject)
                {
                    StopMovement();
                    DrawMessage("Enter Space to continue");
                
                    if (Statistics.LivesCount == MIN_LIVES)
                    {
                        DrawMessage($"GAME OVER, SCORE {Statistics.Score}");
                        await Task.Delay(2000);
                    
                        Statistics = new Statistics
                        {
                            DifficultyLevel = baseStatistics.DifficultyLevel,
                            LivesCount = baseStatistics.LivesCount,
                            Score = baseStatistics.Score,
                        };
                    
                        StartNewGame();
                        return;
                    }
                
                    Statistics.LivesCount--;
                    DrawStatistics(Statistics);
                }

                if (shape is TextObject shape2)
                {
                    Dispatcher.UIThread.Invoke(async () =>
                    {
                        _shapes.Remove(shape2);
                        GameCanvas.Children.Remove(shape2.TextBlock);
                    }, DispatcherPriority.Render);
                }

                if (shape is BaseBonusObject shape3)
                {
                    Dispatcher.UIThread.Invoke(async () =>
                    {
                        _shapes.Remove(shape3);
                        GameCanvas.Children.Remove(shape3.Shape);
                    }, DispatcherPriority.Render);
                }
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

        #region Text
        private void AddTextBox(string text, double X, double Y)
        {
            var box = new TextObject(
                GameCanvas,
                (int) X,
                (int) Y,
                true,
                1,
                text
            )
            {
                AngleSpeed = Double.Pi/2,
                Size = new List<int>{100},
                shouldSkip = false
            };
            Dispatcher.UIThread.Invoke(() =>
            {
                _shapes.Add(box);
                GameCanvas.Children.Add(box.TextBlock);
            });
        }
        
        public void DrawInitStatistics()
        {
            this._statistics = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 28,
                TextWrapping = TextWrapping.Wrap,
                Width = 200,
                Height = 400,
            };

            GameCanvas.Children.Add(_statistics);
        }

        public void DrawStatistics(Statistics statistics)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                _statistics.Text =
                    $"Difficulty: {statistics.DifficultyLevel}\nLives: {statistics.LivesCount}\nScore: {statistics.Score}";

                Canvas.SetRight(_statistics, 5);
                Canvas.SetTop(_statistics, 5);
            }, DispatcherPriority.Render);
        }

        public void InitMessageBlock()
        {
            _fullScreenMessage = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 64,
                TextWrapping = TextWrapping.Wrap,
            };

            GameCanvas.Children.Add(_fullScreenMessage);
        }

        public void DrawMessage(string message)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                this._fullScreenMessage.Text = message;

                Canvas.SetLeft(_fullScreenMessage, 5 );
                Canvas.SetTop(_fullScreenMessage,  5 );
            }, DispatcherPriority.Render);
        }

        #endregion
        
        #region Bonus

        private void AddBonusObject(BaseBonusObject baseBonus)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                _shapes.Add(baseBonus);
                GameCanvas.Children.Add(baseBonus.Shape);
            });
        }

        private void ApplyChangePlatformSize(double value)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                _shapes[0].Shape.Width += value;
                _shapes[0].Size[0] += (int)value;
                _shapes[0].X -= value / 2;
            }, DispatcherPriority.Render);
        }

        private void RemoveChangePlatformSize(double value)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                _shapes[0].Shape.Width -= value;
                _shapes[0].Size[0] -= (int)value;
                _shapes[0].X += value / 2;
            }, DispatcherPriority.Render);
        }

        private void ApplyChangeSpecialBallSpeed(double value)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                _shapes[1].Speed *= value;
            }, DispatcherPriority.Render);
        }
        
        private void RemoveChangeSpecialBallSpeed(double value)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                _shapes[1].Speed /= value;
            }, DispatcherPriority.Render);
        }

        private void ApplyChangeNotSpecBallSpeed(double value)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                foreach (var shape in _shapes)
                {
                    if (!shape.isSpetial && shape is CircleObject)
                    {
                        shape.Speed *= value;
                    }
                }
            }, DispatcherPriority.Render);
        }

        private void RemoveChangeNotSpecBallSpeed(double value)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                foreach (var shape in _shapes)
                {
                    if (!shape.isSpetial && shape is CircleObject)
                    {
                        shape.Speed /= value;
                    }
                }
            }, DispatcherPriority.Render);
        }

        public void ApplyAddNotSpecBalls(int count)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                AddNotSpecShapes(count, (int)GameCanvas.Bounds.Width, (int)GameCanvas.Bounds.Height);
            }, DispatcherPriority.Render);
        }
        
        private void RemoveAddNotSpecBalls(int count)
        {
            // not reset this bonus
        }
        #endregion
        public void StartNewGame()
        {
            ClearCanvas();
            AddRandomShapes(SHAPES_COUNT, (int) GameCanvas.Bounds.Width, (int) GameCanvas.Bounds.Height);
        }
        
        public static (byte,byte,byte) GetRandomBrush()
        {
            Random rand = new Random();
            byte r = (byte)rand.Next(10,250);
            byte g = (byte)rand.Next(10,250);
            byte b = (byte)rand.Next(10,250);

            return (r,g,b);
        }
    }
}