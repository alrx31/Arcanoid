using System.Collections.Generic;
using Avalonia.Controls;

namespace Arcanoid.Models;

public class Platform : RectangleObject
{
    public Platform(
        Canvas canvas,
        int _maxX,
        int _maxY,
        List<int> size,
        byte R1,
        byte G1,
        byte B1,
        byte R2,
        byte G2,
        byte B2,
        bool isSpetial
        ) : base(canvas,
        _maxX,
        _maxY,
        size,
        0,
        R1,
        G1,
        B1,
        R2,
        G2,
        B2,
        isSpetial
        , null)
    {
        
    }

    public override void HandleCollision(DisplayObject other)
    {
        // do nothing
    }
}