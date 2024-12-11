using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sokoban.Enums;
using System;

namespace sokoban;

public class Button
{
    public MainMenuButton Type { get; }
    public Texture2D Texture { get; }
    public int Width { get; }
    public int Height { get; }
    public Vector2 Position { get; set; }
    public Color Color { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsSelected { get; set; }

    public event Action Clicked;

    public Button(MainMenuButton type, Texture2D texture, int width, int height)
    {
        Type = type;
        Texture = texture;
        Width = width;
        Height = height;
        Position = Vector2.Zero;
        Color = Color.Gray;
        IsEnabled = true;
        IsSelected = false;
    }

    public void Update(InputHandler input, MainMenuButton selectedButton)
    {
        IsSelected = selectedButton == Type;

        MouseState mouseState = Mouse.GetState();
        Rectangle buttonRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        if (buttonRectangle.Contains(mouseState.Position))
        {
            Color = Color.White;
            if (mouseState.LeftButton == ButtonState.Pressed && input.IsTurnTime)
            {
                Clicked?.Invoke();
                SoundHandler.Play(Resources.SoundTap);
                input.SetLastMoveTime(input.LastUpdateTime);
            }
        }
        else
            Color = IsSelected ? Color.White : Color.Gray;

        if (!IsEnabled)
            Color *= .7f;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }
}
