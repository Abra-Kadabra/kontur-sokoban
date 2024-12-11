using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sokoban;

public class Text
{
    public SpriteFont Font { get; }
    public string Message { get; private set; }
    public float Length { get; private set; }
    public Color Color { get; }

    public Text(SpriteFont font, string message) : this(font, message, Color.White)
    { }

    public Text(SpriteFont font, string message, Color color)
    {
        Font = font;          
        Color = color;
        Set(message);
    }

    public void Set(string message)
    {
        Message = message;
        Length = Font.MeasureString(Message).Length();
    }
}
