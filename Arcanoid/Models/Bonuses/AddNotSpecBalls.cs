using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Color = Avalonia.Media.Color;

namespace Arcanoid.Models.Bonuses;

public class AddNotSpecBalls : BaseBonusObject
{
    public int CountBalls { get; set; }
    
    public AddNotSpecBalls(Canvas canvas,
        int X,
        int Y,
        double speed,
        int countBalls,
        byte r2,
        byte g2,
        byte b2,
        Action<int> applyAction,
        Action<int> removeAction
        ) : base(canvas,
        X,
        Y,
        speed,
        ()=>applyAction?.Invoke(countBalls),
        ()=>removeAction?.Invoke(countBalls))
    {
        this.Size = new List<int>() { 50, 50 };
        this.CountBalls = countBalls;
        
        Shape = new Rectangle()
        {
            Width = 50,
            Height = 50,
            Fill = new ImageBrush()
            {
                Source = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arcanoid.Assets.AddNotSpecBall.png")),
                Stretch = Stretch.UniformToFill
            },
            Stroke = new SolidColorBrush(Color.FromRgb(r2, g2, b2)),
            StrokeThickness = 1,
            RadiusX = 25,
            RadiusY = 25,
        };
    }
}