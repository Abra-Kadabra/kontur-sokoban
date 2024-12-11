using Microsoft.Xna.Framework;
using sokoban.Enums;

namespace sokoban;

public class GameSession
{
    public readonly Sokoban game;
    public readonly Renderer Renderer;
    public readonly Menu Menu;
    public readonly InputHandler InputHandler;
    public readonly Player Player;

    public Level Level { get; private set; }
    public GameState State { get; set; }

    public GameSession(Sokoban sokoban, Player player, Renderer renderer)
    {
        game = sokoban;
        Player = player;
        Renderer = renderer;
        State = GameState.MENU;
        Menu = new Menu(this);
        InputHandler = new InputHandler(this);
        RestartLevel();
    }

    public void RestartLevel()
    {
        Level = new Level(this, Maps.Get());
        Renderer.AdaptToLevel(Level);
        InputHandler.AdaptToLevel(Level);
    }

    public void Update(GameTime gameTime)
    {
        game.IsMouseVisible = State == GameState.MENU && Menu.IsMainMenu;

        InputHandler.Update(gameTime);

        if (State == GameState.MENU)
            Menu.Update(gameTime);
    }

    public void Draw()
    {
        Renderer.Draw(this);
    }
}
