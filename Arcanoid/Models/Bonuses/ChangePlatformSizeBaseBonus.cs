using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Arcanoid.Models.Bonuses;

public class ChangePlatformSizeBaseBonus : BaseBonusObject
{
    public int ChangeSize { get; set; }
    
    public ChangePlatformSizeBaseBonus(Canvas canvas,
        int X,
        int Y,
        double speed,
        int changeSizeValue,
        byte r1,
        byte g1,
        byte b1,
        byte r2,
        byte g2,
        byte b2,
        Action<double> applyAction,
        Action<double> removeAction
        ) : base(canvas,
        X,
        Y,
        speed,
        () => applyAction?.Invoke(changeSizeValue), // callback for bonus apply
        () => removeAction?.Invoke(changeSizeValue) // callback for bonus remove
        )
    {
        this.Size = new List<int>() { 50, 50 };
        ChangeSize = changeSizeValue;

        Shape = new Rectangle()
        {
            Width = 50,
            Height = 50,
            Fill = new SolidColorBrush(Color.FromRgb(r1, g1, b1)),
            Stroke = new SolidColorBrush(Color.FromRgb(r2, g2, b2)),
            StrokeThickness = 1
        };
    }
}