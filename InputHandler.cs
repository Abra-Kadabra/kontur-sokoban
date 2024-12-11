using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using sokoban.Enums;
using System;

using static sokoban.Resources;

namespace sokoban;

public class InputHandler
{
    public readonly TimeSpan TurnDelay = TimeSpan.FromMilliseconds(180);
    public readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(160);

    public TimeSpan LastUpdateTime { get; private set; }
    public TimeSpan LastMoveTime { get; private set; }
    public TimeSpan ElapsedSinceMoveTime { get { return LastUpdateTime - LastMoveTime; } }
    public TimeSpan LevelStartTime { get; private set; }
    public TimeSpan LevelPausedTime { get; private set; }
    public TimeSpan LevelTime { get { return LastUpdateTime - LevelStartTime - LevelPausedTime; } }
    public bool IsTurnTime { get { return LastUpdateTime - LastMoveTime >= TurnDelay; } }
    public int TurnsCount { get; private set; }
    public bool HideStatusInfo { get; private set; }

    private readonly GameSession game;
    private Level level;

    private bool lastUpdateWasPlay;

    public InputHandler(GameSession session)
    {
        game = session;
    }

    public void AdaptToLevel(Level map)
    {
        level = map;
        TurnsCount = 0;
        LevelPausedTime = TimeSpan.Zero;
        LevelStartTime = TimeSpan.FromMilliseconds(LastUpdateTime.TotalMilliseconds);
    }

    public void Update(GameTime gameTime)
    {
        if (!lastUpdateWasPlay)
            LevelPausedTime += gameTime.TotalGameTime - LastUpdateTime;

        lastUpdateWasPlay = game.State == GameState.PLAY;
        LastUpdateTime = gameTime.TotalGameTime;

        var kstate = Keyboard.GetState();

        switch (game.State)
        {
            case GameState.MENU when IsTurnTime:
                lastUpdateWasPlay = false;
                game.Menu.Update(kstate, gameTime);
                break;
            case GameState.PLAY when IsTurnTime:
                lastUpdateWasPlay = true;
                ProcessTurn(kstate, gameTime);
                break;
            case GameState.PAUSE when IsTurnTime && kstate.IsKeyDown(Keys.Space):
                game.State = GameState.PLAY;
                SetLastMoveTime(gameTime.TotalGameTime);
                break;
            case GameState.HELP when IsTurnTime && (kstate.IsKeyDown(Keys.F1) || kstate.IsKeyDown(Keys.Escape)):
                game.State = GameState.PLAY;
                SetLastMoveTime(gameTime.TotalGameTime);
                break;
        }
    }

    public void SetLastMoveTime(TimeSpan time)
    {
        LastMoveTime = time;
    }

    private void ProcessTurn(KeyboardState kstate, GameTime gameTime)
    {
        if (kstate.IsKeyDown(Keys.Escape))
            game.State = GameState.MENU;
        else if (kstate.IsKeyDown(Keys.Space))
            game.State = GameState.PAUSE;
        else if (kstate.IsKeyDown(Keys.F1))
            game.State = GameState.HELP;
        else if (kstate.IsKeyDown(Keys.R))
            game.RestartLevel();
        else if (kstate.IsKeyDown(Keys.M))
            SoundHandler.MuteMusic = !SoundHandler.MuteMusic;
        else if (kstate.IsKeyDown(Keys.S))
            SoundHandler.MuteSound = !SoundHandler.MuteSound;
        else if (kstate.IsKeyDown(Keys.Z))
            IsSeriousTheme = !IsSeriousTheme;
        else if (kstate.IsKeyDown(Keys.H))
            HideStatusInfo = !HideStatusInfo;
        else if (kstate.IsKeyDown(Keys.Up))
            MovePlayer(Direction.UP);
        else if (kstate.IsKeyDown(Keys.Down))
            MovePlayer(Direction.DOWN);
        else if (kstate.IsKeyDown(Keys.Right))
            MovePlayer(Direction.RIGTH);
        else if (kstate.IsKeyDown(Keys.Left))
            MovePlayer(Direction.LEFT);
        else
            return;

        SetLastMoveTime(gameTime.TotalGameTime);
    }

    private bool MovePlayer(Direction direction)
    {
        var x = level.PlayerActor.Position.X;
        var y = level.PlayerActor.Position.Y;

        switch (direction)
        {
            case Direction.DOWN:
                y += 1; break;
            case Direction.UP:
                y -= 1; break;
            case Direction.LEFT:
                x -= 1; break;
            case Direction.RIGTH:
                x += 1; break;
        }

        if (!CanMove(direction, x, y))
        {
            level.PlayerActor.Direction = direction;
            return false;
        }

        MoveActor(level.PlayerActor, direction, x, y);
        SoundHandler.Play(SoundStep);
        TurnsCount++;
        return true;
    }

    private void MoveActor(Actor actor, Direction direction, int x, int y)
    {
        actor.IsMoving = true;
        actor.PreviousPosition = (Coordinates)actor.Position.Clone();
        actor.Direction = direction;
        actor.Position = new Coordinates(x, y);
    }

    private bool CanMove(Direction direction, int x, int y)
    {
        if (x < 0 || y < 0 || x >= level.Width || y >= level.Height)
            return false;

        foreach (var crate in level.Crates)
            if (crate.Position.X == x && crate.Position.Y == y)
                return TryMoveCrate(crate, direction, x, y);

        return level.Map[x, y].Terrain != Terrain.WALL;
    }

    private bool TryMoveCrate(Actor theCrate, Direction direction, int x, int y)
    {
        var newX = x;
        var newY = y;

        switch (direction)
        {
            case Direction.DOWN:
                newY += 1; break;
            case Direction.UP:
                newY -= 1; break;
            case Direction.LEFT:
                newX -= 1; break;
            case Direction.RIGTH:
                newX += 1; break;
        }

        if (newX < 0 || newY < 0 || newX >= level.Width || newY >= level.Height)
            return false;

        if (level.Map[newX, newY].Terrain == Terrain.WALL)
            return false;

        foreach (var crate in level.Crates)
            if (crate.Position.X == newX && crate.Position.Y == newY)
                return false;

        if (level.Map[newX, newY].Terrain == Terrain.TARGET)
            SoundHandler.Play(SoundTap);

        SoundHandler.Play(SoundBox);

        MoveActor(theCrate, direction, newX, newY);
        return true;
    }
}
