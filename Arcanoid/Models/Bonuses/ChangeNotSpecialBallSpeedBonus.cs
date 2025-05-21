using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Arcanoid.Models.Bonuses;

public class ChangeNotSpecialBallSpeedBonus : BaseBonusObject
{
    public double ChangeSpeedValue { get; set; }
    
    public ChangeNotSpecialBallSpeedBonus(Canvas canvas,
        int X,
        int Y,
        double speed,
        double changeSpeedValue,
        byte r2,
        byte g2,
        byte b2,
        Action<double> applyAction,
        Action<double> removeAction) : base(canvas,
        X,
        Y,
        speed,
        ()=>applyAction?.Invoke(changeSpeedValue), 
        ()=>removeAction?.Invoke(changeSpeedValue)
        )
    {
        this.Size = new List<int>() { 50, 50 };
        this.ChangeSpeedValue = changeSpeedValue;
        
        Shape = new Rectangle()
        {
            Width = 50,
            Height = 50,
            Fill = new ImageBrush()
            {
                Source = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arcanoid.Assets.SpeedupNotSpecBall.png")),
                Stretch = Stretch.UniformToFill
            },
            Stroke = new SolidColorBrush(Color.FromRgb(r2, g2, b2)),
            StrokeThickness = 1,
            RadiusX = 25,
            RadiusY = 25,
        };
    }
}