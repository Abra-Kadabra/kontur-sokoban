using sokoban.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sokoban;

public class Level : ICloneable
{
    public Cell[,] Map { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public List<Cell> Targets { get; private set; }
    public List<Actor> Crates { get; private set; }
    public Actor PlayerActor { get; private set; }
    public bool IsSolved
    {
        get
        {
            var solved = Targets.All(cell => Crates.Any(crate => cell.Position.Equals(crate.Position)));
            if (solved)
            {
                var record = Maps.GetRecords();
                if (record.Steps > game.InputHandler.TurnsCount)
                {
                    record.Steps = game.InputHandler.TurnsCount;
                    Maps.SaveRecords();
                }
                if (record.Time > game.InputHandler.LevelTime)
                {
                    record.Time = game.InputHandler.LevelTime;
                    Maps.SaveRecords();
                }
            }
            return solved;
        }
    }

    private GameSession game;
    private string[] originMap;

    public Level(GameSession session, string[] map)
    {
        game = session;
        originMap = map;
        PlayerActor = new Actor(Agent.PLAYER, new Coordinates(-1, -1));
        Map = Construct(map);
    }

    public object Clone()
    {
        return new Level(game, originMap);
    }

    public Cell[,] Construct(string[] map)
    {
        Height = map.Length;
        Width = map[0].Length;

        if (Height < 3 || Width < 3)
            throw new ArgumentException("The map is too small.");

        // TODO карта не может быть больше размера экрана. а также меньше чем требуется для корректного отображения меню. 
        //if (DisplayMode.Width GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height

        var result = new Cell[Width, Height];
        var playerSet = false;

        Targets = new List<Cell>();
        Crates = new List<Actor>();

        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var cell = CellFromChar(map[y][x], x, y);
                var agent = AgentFromChar(map[y][x], x, y);

                result[x, y] = cell;

                if (cell.Terrain == Terrain.TARGET)
                    Targets.Add(cell);

                if (agent == Agent.PLAYER)
                {
                    if (playerSet)
                        throw new ArgumentException("The Player already exists on the map.");

                    playerSet = true;
                    PlayerActor.Position.X = x;
                    PlayerActor.Position.Y = y;
                }
            }

        if (Targets.Count == 0)
            throw new ArgumentException("No targets found on the map.");

        if (Targets.Count > Crates.Count)
            throw new ArgumentException("Not enough crates on the map to solve all the targets.");

        return result;
    }

    public Cell CellFromChar(char ch, int x, int y)
    {
        Terrain terrain;
        var position = new Coordinates(x, y);

        switch (ch)
        {
            case '#':
                terrain = Terrain.WALL;
                break;
            case 'X':
                terrain = Terrain.TARGET;
                break;
            case '~':
                terrain = Terrain.ABYSS;
                break;
            case '.':
            case '@':
            case 'O':
                terrain = Terrain.FLOOR;
                break;
            default:
                throw new NotImplementedException();
        }
        return new Cell(terrain, position);
    }

    public Agent AgentFromChar(char ch, int x, int y)
    {
        var agent = Agent.NONE;
        var position = new Coordinates(x, y);

        switch (ch)
        {
            case '@':
                agent = Agent.PLAYER;
                PlayerActor.Position = position;
                break;
            case 'O':
                agent = Agent.CRATE;
                Crates.Add(new Actor(Agent.CRATE, position));
                break;
            default:
                break;
        }
        return agent;
    }
}
