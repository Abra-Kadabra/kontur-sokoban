using System;
using Microsoft.Xna.Framework;

namespace sokoban;

public class Coordinates : ICloneable, IEquatable<Coordinates>
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2 ToVector()
    {
        return new Vector2(X * Resources.SpriteSize, Y * Resources.SpriteSize);
    }

    public object Clone()
    {
        return new Coordinates(X, Y);
    }

    public bool Equals(Coordinates other)
    {
        if (other == null)
            return false;

        return this.X == other.X && this.Y == other.Y;
    }
}
