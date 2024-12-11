using sokoban.Enums;
using System;

namespace sokoban;

public class Cell : ICloneable
{
    public Terrain Terrain { get; set; }

    public Coordinates Position { get; set; }

    public Cell(Terrain terrain, Coordinates position)
    {
        Terrain = terrain;
        Position = position;
    }

    public object Clone()
    {
        return new Cell(Terrain, (Coordinates)Position.Clone());
    }
}
