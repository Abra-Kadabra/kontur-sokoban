using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace sokoban;

public class Sokoban : Game
{
    public static Logger Logger;
    public GameSession Session;

    public Sokoban()
    {
        Logger = new Logger();
        Content.RootDirectory = "Content";

        // TODO поддержка состояний игрока между сессиями, загрузка/сохрание настрек, прогресса и пр.
        var player = new Player("Anon");
        var Renderer = new Renderer(new GraphicsDeviceManager(this));
        Session = new GameSession(this, player, Renderer);
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        Resources.Init(Content);
        Session.Renderer.SetSpriteBatch(new SpriteBatch(GraphicsDevice));
        Session.Menu.LoadMapNames();
        Session.Menu.InitButtonClicks();
        SoundHandler.RestartMusic();
    }

    protected override void Update(GameTime gameTime)
    {
        Session.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gray);
        Session.Draw();
        base.Draw(gameTime);
    }
}
