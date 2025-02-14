using System;
using System.Collections.Generic;
using Arcanoid.Models;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace Arcanoid;

public class Stage
{
    public Canvas Canvas { get; private set; }
    private readonly List<DisplayObject> _shapes;
    private readonly DispatcherTimer _timer;

    public Stage()
    {
        Canvas = new Canvas
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

    public void AddRandomShapes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _shapes.Add(new CircleObject(Canvas));
            _shapes.Add(new RectangleObject(Canvas));
            _shapes.Add(new TriangleShape(Canvas));
            _shapes.Add(new TrapezoidObject(Canvas));
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
    
    private void OnTimerTick(object sender, EventArgs e)
    {
        foreach (var shape in _shapes)
        {
            shape.Move();
        }
    }
}