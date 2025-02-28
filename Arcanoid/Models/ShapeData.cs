namespace Arcanoid.Models;


public class ShapeData
{
    public string ShapeType { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public double Speed { get; set; }
    public double Angle { get; set; }
    public double Acceleration { get; set; }
    public int Size { get; set; }
    
    public byte R1 { get; set; }
    public byte G1 { get; set; }
    public byte B1 { get; set; }
    public byte R2 { get; set; }
    public byte G2 { get; set; }
    public byte B2 { get; set; }
}