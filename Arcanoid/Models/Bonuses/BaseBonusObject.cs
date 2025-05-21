using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Arcanoid.Models;

public abstract class BaseBonusObject : DisplayObject
{
    private const int BONUS_DURATION = 10_000; // 10 sec bonus duration

    private readonly Action _applyAction;
    private readonly Action _removeAction;
    
    public BaseBonusObject(Canvas canvas,
        int X,
        int Y,
        double speed,
        Action applyAction,
        Action removeAction
        ) : base(canvas, X, Y, true, speed, null)
    {
        shouldSkip = true;
        _applyAction = applyAction;
        _removeAction = removeAction;
    }

    public async override void HandleCollision(DisplayObject other)
    {
        if (other is Platform)
        {
            ApplyBonus();
            await Task.Delay(BONUS_DURATION);
            RemoveBonus();
        }
        
        // other not handle
    }

    public void ApplyBonus()
    {
        _applyAction?.Invoke();
    }

    public void RemoveBonus()
    {
        _removeAction?.Invoke();
    }

    public override void Draw()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            Canvas.SetLeft(Shape, X );
            Canvas.SetTop(Shape,  Y );
        }, DispatcherPriority.Render);
    }
}