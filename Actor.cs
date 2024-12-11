using sokoban.Enums;
using System;

namespace sokoban;

public class Actor : ICloneable
{
    public Agent Agent { get; set; }
    public Coordinates Position { get; set; }
    public Direction Direction { get; set; }
    public bool IsMoving { get; set; }
    public Coordinates PreviousPosition { get; set; }

    public Actor(Agent agent, Coordinates position, Direction direction = Direction.DOWN)
    {
        Agent = agent;
        Position = position;
        Direction = direction;
    }

    public object Clone()
    {
        return new Actor(Agent, (Coordinates)Position.Clone(), Direction);
    }
}
