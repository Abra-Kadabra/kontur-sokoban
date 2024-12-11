using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using sokoban.Enums;
using System.Collections.Generic;

using static sokoban.Resources;

namespace sokoban;

public class Menu
{
    public const int MainMenuRows = 4;

    public MainMenuButton ActiveMainMenu;

    public bool IsMainMenu = true;

    private readonly GameSession game;
    private Dictionary<string, Text> mapNames;
    public int SettingsMenuButtons { get { return mapNames.Count; } }
    public int ActiveSettingsMenu { get; private set; }

    private readonly List<MainMenuButton> settingsRow = new() {
        MainMenuButton.SOUND,
        MainMenuButton.MAPS,
        MainMenuButton.MUSIC
    };

    public Menu(GameSession session)
    {
        game = session;
        ActiveMainMenu = MainMenuButton.RESUME;
    }

    public void InitButtonClicks()
    {
        NewGameButton.Clicked += OnNewGameButtonClicked;
        ResumeButton.Clicked += OnResumeButtonClicked;
        MapsButton.Clicked += OnMapsButtonClicked;
        MusicButton.Clicked += OnMusicButtonClicked;
        SoundButton.Clicked += OnSoundButtonClicked;
        ExitButton.Clicked += OnExitButtonClicked;
    }

    private void OnResumeButtonClicked()
    {
        game.State = GameState.PLAY;
    }

    public void OnNewGameButtonClicked()
    {
        game.RestartLevel();
        game.State = GameState.PLAY;
    }

    private void OnMapsButtonClicked()
    {
        IsMainMenu = false;
    }

    private void OnMusicButtonClicked()
    {
        SoundHandler.MuteMusic = !SoundHandler.MuteMusic;
    }

    private void OnSoundButtonClicked()
    {
        SoundHandler.MuteSound = !SoundHandler.MuteSound;
    }

    private void OnExitButtonClicked()
    {
        game.game.Exit();
    }

    public void Update(GameTime gameTime)
    {
        NewGameButton.Update(game.InputHandler, ActiveMainMenu);
        ResumeButton.Update(game.InputHandler, ActiveMainMenu);
        MapsButton.Update(game.InputHandler, ActiveMainMenu);

        MusicButton.Update(game.InputHandler, ActiveMainMenu);
        MusicButton.IsEnabled = !SoundHandler.MuteMusic;

        SoundButton.Update(game.InputHandler, ActiveMainMenu);
        SoundButton.IsEnabled = !SoundHandler.MuteSound;

        ExitButton.Update(game.InputHandler, ActiveMainMenu);
    }

    public void Update(KeyboardState kstate, GameTime gameTime)
    {
        if (kstate.IsKeyDown(Keys.Escape))
        {
            game.State = GameState.PLAY;
            IsMainMenu = true;
            ActiveMainMenu = MainMenuButton.RESUME;
        }
        else if (kstate.IsKeyDown(Keys.Space) || kstate.IsKeyDown(Keys.Enter))
            ActivateCurrentButton();
        else if (kstate.IsKeyDown(Keys.Up))
            ChangeActiveButton(Direction.UP);
        else if (kstate.IsKeyDown(Keys.Down))
            ChangeActiveButton(Direction.DOWN);
        else if (kstate.IsKeyDown(Keys.Left))
            ChangeActiveButtonHorisontal(Direction.LEFT);
        else if (kstate.IsKeyDown(Keys.Right))
            ChangeActiveButtonHorisontal(Direction.RIGTH);
        else
            return;
        game.InputHandler.SetLastMoveTime(gameTime.TotalGameTime);
    }

    private void ChangeActiveButton(Direction direction)
    {
        int indexShift = 0;
        switch (direction)
        {
            case Direction.UP:
                indexShift -= 1;
                break;
            case Direction.DOWN:
                indexShift += 1;
                break;
            default:
                break;
        }

        if (IsMainMenu)
            ActiveMainMenu = (MainMenuButton)(((int)ActiveMainMenu + indexShift + MainMenuRows) % MainMenuRows);
        else
            ActiveSettingsMenu = (ActiveSettingsMenu + indexShift + SettingsMenuButtons) % SettingsMenuButtons;
    }

    private void ChangeActiveButtonHorisontal(Direction direction)
    {
        if (!IsMainMenu || !settingsRow.Contains(ActiveMainMenu))
            return;

        int index = settingsRow.IndexOf(ActiveMainMenu);

        switch (direction)
        {
            case Direction.LEFT:
                index -= 1;
                break;
            case Direction.RIGTH:
                index += 1;
                break;
            default:
                break;
        }

        if (index < 0)
            index = settingsRow.Count - 1;
        else if (index >= settingsRow.Count)
            index = 0;

        ActiveMainMenu = settingsRow[index];
    }

    private void ActivateCurrentButton()
    {
        if (IsMainMenu)
        {
            switch (ActiveMainMenu)
            {
                case MainMenuButton.RESUME:
                    OnResumeButtonClicked();
                    break;
                case MainMenuButton.NEW_GAME:
                    OnNewGameButtonClicked();
                    break;
                case MainMenuButton.SOUND:
                    OnSoundButtonClicked();
                    return;
                case MainMenuButton.MAPS:
                    OnMapsButtonClicked();
                    break;
                case MainMenuButton.MUSIC:
                    OnMusicButtonClicked();
                    return;
                case MainMenuButton.EXIT:
                    OnExitButtonClicked();
                    break;
            }
        }
        else
        {
            Maps.active = Maps.Atlas[ActiveSettingsMenu];
            game.RestartLevel();

            IsMainMenu = true;
        }
    }
    public void LoadMapNames()
    {
        mapNames = new Dictionary<string, Text>();
        foreach (var map in Maps.Atlas)
        {
            Sokoban.Logger.Debug($"Menu loaded map: {map}");
            mapNames[map] = new Text(MenuFont, map);
        }
    }

    public Text GetMapText(string name)
    {
        return mapNames[name];
    }
}
