using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sokoban.Enums;
using System;

using static sokoban.Resources;

namespace sokoban;

public class Renderer
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch sprites;

    public Renderer(GraphicsDeviceManager graphicsManager)
    {
        graphics = graphicsManager;
        graphics.PreferredBackBufferWidth = 7 * SpriteSize;
        graphics.PreferredBackBufferHeight = 7 * SpriteSize;
    }

    public void SetSpriteBatch(SpriteBatch spriteBatch)
    {
        sprites = spriteBatch;
    }

    public void AdaptToLevel(Level level)
    {
        // TODO адаптивный дизайн. минимальная ширина = статусбар. минимальная высота = меню.
        graphics.PreferredBackBufferWidth = level.Width * SpriteSize;
        graphics.PreferredBackBufferHeight = level.Height * SpriteSize;
        graphics.ApplyChanges();
    }

    public void Draw(GameSession game)
    {
        sprites.Begin();
        DrawLevel(game, game.InputHandler);
        DrawState(game.State, game.Menu, game.Level.IsSolved);
        sprites.End();
    }

    private void DrawLevel(GameSession game, InputHandler input)
    {
        var level = game.Level;
        var map = level.Map;
        for (var y = 0; y < level.Height; y++)
            for (var x = 0; x < level.Width; x++)
            {
                var terrainTexture = GetTerrainTexture(map[x, y]);
                DrawSprite(terrainTexture, x, y);
            }

        foreach (var crate in level.Crates)
            DrawActorSprite(crate, level, input);

        DrawActorSprite(level.PlayerActor, level, input);

        if (game.State != GameState.MENU && !game.InputHandler.HideStatusInfo)
            DrawStatusBar(input);
    }

    private Texture2D GetTerrainTexture(Cell cell)
    {
        switch (cell.Terrain)
        {
            // TODO ITheme { GetWallTexture; GetFlortexture; ... } 
            case Terrain.WALL:
                return IsSeriousTheme ? WallSeriousTexture : WallTexture;
            case Terrain.FLOOR:
                return IsSeriousTheme ? FloorSeriousTexture : FloorTexture;
            case Terrain.TARGET:
                return IsSeriousTheme ? TargetSeriousTexture : TargetTexture;
            case Terrain.ABYSS:
                return AbyssTexture;
            default:
                return ErrorTexture;
        }
    }

    private void DrawActorSprite(Actor actor, Level level, InputHandler input)
    {
        var texture = actor.Agent == Agent.PLAYER ? GetPlayerTexture(actor) : GetCrateTexture(actor, level);

        if (input.ElapsedSinceMoveTime.TotalMilliseconds >= input.AnimationDuration.TotalMilliseconds && actor.IsMoving)
            actor.IsMoving = false;

        if (actor.IsMoving)
        {
            var progress = EaseInOut(input.ElapsedSinceMoveTime.TotalMilliseconds / input.AnimationDuration.TotalMilliseconds);
            var interpolatedPosition = Vector2.Lerp(actor.PreviousPosition.ToVector(), actor.Position.ToVector(), progress);
            sprites.Draw(texture, interpolatedPosition, Color.White);
        }
        else
            DrawSprite(texture, actor.Position.X, actor.Position.Y);
    }

    private float EaseInOut(double t)
    {
        return 0.5f * (1 - (float)Math.Cos(t * Math.PI));
    }

    private Texture2D GetPlayerTexture(Actor actor)
    {
        if (actor.Agent != Agent.PLAYER)
            throw new ArgumentException("GetPlayerTexture for Agent.PLAYER only."); // TODO polymorph it

        return PlayerTexture[(int)actor.Direction];
    }

    private Texture2D GetCrateTexture(Actor actor, Level level)
    {
        if (actor.Agent != Agent.CRATE)
            throw new ArgumentException("GetCrateTexture for Agent.CRATE only."); // TODO polymorph it

        var index = GetActorCell(level.Map, actor).Terrain == Terrain.TARGET && !actor.IsMoving ? 1 : 0;
        return IsSeriousTheme ? CrateSeriousTexture[index] : CrateTexture[index];
    }

    private void DrawSprite(Texture2D texture, int x, int y)
    {
        var position = new Vector2(x * SpriteSize, y * SpriteSize);
        sprites.Draw(texture, position, Color.White);
    }

    private Cell GetActorCell(Cell[,] map, Actor actor)
    {
        return map[actor.Position.X, actor.Position.Y];
    }

    public void DrawState(GameState state, Menu menu, bool isSolved)
    {
        switch (state)
        {
            case GameState.MENU:
                if (menu.IsMainMenu)
                    DrawMainMenu(menu.ActiveMainMenu);
                else
                    DrawMapsMenu(menu);
                break;
            case GameState.PLAY:
                if (isSolved)
                    DrawText(TextSolved);
                break;
            case GameState.PAUSE:
                DrawText(TextPause);
                break;
            case GameState.HELP:
                DrawF1Help();
                break;
        }
    }

    private void DrawText(Text text, int yOffset = 45, bool selected = false)
    {
        var textX = graphics.PreferredBackBufferWidth / 2 - text.Length / 2;
        var textY = graphics.PreferredBackBufferHeight / 2 + yOffset;
        DrawText(text, new Vector2(textX, textY), selected);
    }

    private void DrawText(Text text, Vector2 position, bool selected = false)
    {
        sprites.DrawString(text.Font, text.Message, position + new Vector2(1, 1), Color.Black);
        sprites.DrawString(text.Font, text.Message, position + new Vector2(-1, 1), Color.Black);

        sprites.DrawString(text.Font, text.Message, position, selected ? Color.Yellow : text.Color);
    }

    public void DrawMainMenu(MainMenuButton activeButton)
    {
        // TODO перевести ответственность в Button
        var menuHeigth = Menu.MainMenuRows * (UsedButtonHeigth + SpaceBetweenButtons) - SpaceBetweenButtons;

        var resumeButtonY = graphics.PreferredBackBufferHeight / 2 - menuHeigth / 2;
        DrawButton(ResumeButton, resumeButtonY);

        var newGameButtonY = resumeButtonY + SpaceBetweenButtons + UsedButtonHeigth;
        DrawButton(NewGameButton, newGameButtonY);

        var smallButtonsY = newGameButtonY + SpaceBetweenButtons + UsedButtonHeigth;
        DrawButton(SoundButton, smallButtonsY);
        DrawButton(MapsButton, smallButtonsY, UsedButtonHeigth);
        DrawButton(MusicButton, smallButtonsY, 2 * UsedButtonHeigth);

        var exitButtonY = smallButtonsY + SpaceBetweenButtons + UsedButtonHeigth;
        DrawButton(ExitButton, exitButtonY);
    }

    private void DrawButton(Button button, int y, int offset = 0)
    {
        var x = graphics.PreferredBackBufferWidth / 2 - UsedButtonWidth / 2 + offset;
        button.Position = new Vector2(x, y);
        DrawButton(button.Texture, button.Position, button.Color);
    }

    private void DrawButton(Texture2D button, Vector2 position, Color color)
    {
        var scale = 1.0f * UsedButtonWidth / OriginalButtonWidth;
        sprites.Draw(button, position, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    public void DrawMapsMenu(Menu menu)
    {
        DrawText(TextSelectMap, -150);
        for (var i = 0; i < Maps.Atlas.Count; i++)
        {
            var map = Maps.Atlas[i];
            if (i == menu.ActiveSettingsMenu)
                DrawText(TextUnder, i * 28 - 95, map == Maps.active);
            DrawText(menu.GetMapText(map), i * 28 - 100, map == Maps.active);
        }
    }

    private const int margin = 5;
    private const int rowHeigth = 20;
    private const int betweenRows = 28;

    private void DrawStatusBar(InputHandler input)
    {
        TextTime.Set(FormatTimes(input.LevelTime, "Time"));
        DrawText(TextTime, new Vector2(margin, margin));
        TextTime.Set(FormatTimes(Maps.RecordsMap[Maps.active].Time, "Best"));
        DrawText(TextTime, new Vector2(margin, margin + rowHeigth));

        TextSteps.Set(FormatSteps(input.TurnsCount, "Steps"));
        DrawText(TextSteps, new Vector2(graphics.PreferredBackBufferWidth - TextSteps.Length, margin));
        TextSteps.Set(FormatSteps(Maps.RecordsMap[Maps.active].Steps, "Best"));
        DrawText(TextSteps, new Vector2(graphics.PreferredBackBufferWidth - TextSteps.Length, margin + rowHeigth));

        TextLevel.Set($"Level: {Maps.active}");
        DrawText(TextLevel, new Vector2(margin, graphics.PreferredBackBufferHeight - (margin + rowHeigth)));

        DrawText(TextHelp, new Vector2(graphics.PreferredBackBufferWidth - TextHelp.Length, graphics.PreferredBackBufferHeight - (margin + rowHeigth)));
    }

    private string FormatTimes(TimeSpan time, string name)
    {
        return $"{name}: {time.Minutes:D2}:{time.Seconds:D2}";
    }

    private string FormatSteps(int steps, string name)
    {
        var best = Maps.RecordsMap[Maps.active].Steps;
        return $"{name}: {steps:D4}";
    }

    private void DrawF1Help()
    {
        var y = -120;
        foreach (var text in TextHotkeys)
        {
            DrawText(text, y);
            y += betweenRows;
        }
    }
}

