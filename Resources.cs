using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using sokoban.Enums;

namespace sokoban;

public static class Resources
{
    public const int SpriteSize = 64;

    public const int SpaceBetweenButtons = 5;
    public const int OriginalButtonWidth = 600;
    public const int OriginalButtonHeight = 200;
    public const int UsedButtonWidth = 200;
    public const int UsedButtonHeigth = 67;

    //public static Button MenuButton;
    public static Button NewGameButton;
    public static Button ResumeButton;
    public static Button MapsButton;
    public static Button MusicButton;
    public static Button SoundButton;
    public static Button ExitButton;

    public static Texture2D FloorTexture;
    public static Texture2D TargetTexture;
    public static Texture2D WallTexture;
    public static Texture2D AbyssTexture;
    public static Texture2D[] CrateTexture;

    public static Texture2D FloorSeriousTexture;
    public static Texture2D TargetSeriousTexture;
    public static Texture2D WallSeriousTexture;
    public static Texture2D AbyssSeriousTexture;
    public static Texture2D[] CrateSeriousTexture;

    public static Texture2D[] PlayerTexture;
    public static Texture2D ErrorTexture;

    public static Song PopcornSong;
    public static Song TimeSong;

    public static SoundEffect SoundTap;
    public static SoundEffect SoundStep;
    public static SoundEffect SoundBox;

    public static SpriteFont MenuFont;

    public static Text TextSelectMap;
    public static Text TextPause;
    public static Text TextSolved;
    public static Text TextUnder;

    public static Text TextTime;
    public static Text TextSteps;
    public static Text TextLevel;
    public static Text TextHelp;

    public static Text[] TextHotkeys;

    private static bool _isSerious;
    public static bool IsSeriousTheme { get { return _isSerious; } set { _isSerious = value; SoundHandler.RestartMusic(); } }

    public static void Init(ContentManager Content)
    {
        //MenuButton = new Button(MainMenuButton.EXIT, Content.Load<Texture2D>("button_menu"), UsedButtonWidth, UsedButtonHeigth);
        NewGameButton = new Button(MainMenuButton.NEW_GAME, Content.Load<Texture2D>("button_new_game"), UsedButtonWidth, UsedButtonHeigth);
        ResumeButton = new Button(MainMenuButton.RESUME, Content.Load<Texture2D>("button_resume"), UsedButtonWidth, UsedButtonHeigth);
        MapsButton = new Button(MainMenuButton.MAPS, Content.Load<Texture2D>("button_maps"), UsedButtonHeigth, UsedButtonHeigth);
        MusicButton = new Button(MainMenuButton.MUSIC, Content.Load<Texture2D>("button_music"), UsedButtonHeigth, UsedButtonHeigth);
        SoundButton = new Button(MainMenuButton.SOUND, Content.Load<Texture2D>("button_sound"), UsedButtonHeigth, UsedButtonHeigth);
        ExitButton = new Button(MainMenuButton.EXIT, Content.Load<Texture2D>("button_exit"), UsedButtonWidth, UsedButtonHeigth);

        FloorSeriousTexture = Content.Load<Texture2D>("floor");
        FloorTexture = Content.Load<Texture2D>("ground");
        TargetSeriousTexture = Content.Load<Texture2D>("target");
        TargetTexture = Content.Load<Texture2D>("ground_x");
        WallSeriousTexture = Content.Load<Texture2D>("wall-brick");
        WallTexture = Content.Load<Texture2D>("wall");
        AbyssTexture = Content.Load<Texture2D>("abyss-dragons");
        CrateTexture = new Texture2D[] {
            Content.Load<Texture2D>("crate_missed"),
            Content.Load<Texture2D>("crate_placed"),
        };
        CrateSeriousTexture = new Texture2D[] {
            Content.Load<Texture2D>("crate-active"),
            Content.Load<Texture2D>("crate-passive"),
        };
        PlayerTexture = new Texture2D[] {
            Content.Load<Texture2D>("player_n"),
            Content.Load<Texture2D>("player_e"),
            Content.Load<Texture2D>("player_s"),
            Content.Load<Texture2D>("player_w")
        };
        ErrorTexture = Content.Load<Texture2D>("error");

        PopcornSong = Content.Load<Song>("popcorn");
        TimeSong = Content.Load<Song>("time");

        SoundTap = Content.Load<SoundEffect>("switch-tap");
        SoundStep = Content.Load<SoundEffect>("step");
        SoundBox = Content.Load<SoundEffect>("box");

        MenuFont = Content.Load<SpriteFont>("fonts/lucida");

        TextSelectMap = new Text(MenuFont, "Select map to play:");
        TextPause = new Text(MenuFont, "Press Space to Continue");
        TextSolved = new Text(MenuFont, "You won!", Color.Yellow);
        TextUnder = new Text(MenuFont, "_____");

        TextTime = new Text(MenuFont, "Time: 00:00.00");
        TextSteps = new Text(MenuFont, "Steps: 0000");
        TextLevel = new Text(MenuFont, "Level: map0");
        TextHelp = new Text(MenuFont, "F1 for hotkeys");

        TextHotkeys = new Text[]
        {
            new Text(MenuFont, "Arrows - move character"),
            new Text(MenuFont, "R - restart level"),
            new Text(MenuFont, "M - switch music"),
            new Text(MenuFont, "S - switch sound"),
            new Text(MenuFont, "Z - switch theme"),
            new Text(MenuFont, "H - switch status info"),
            new Text(MenuFont, "Space - switch pause"),
            new Text(MenuFont, "F1 - switch this help"),
            new Text(MenuFont, "Esc - open menu"),
        };
    }
}
