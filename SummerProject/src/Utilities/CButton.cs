using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SummerProject
{
    class CButton
    {
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;

        Color colour = new Color(255, 255, 255, 255);

        public Vector2 Size;

        bool down;
        public bool isClicked;

        public CButton(Texture2D newTexture, GraphicsDevice graphics)
        {
            texture = newTexture;

            //ScreenW = 800, ScreenH = 600
            //ImgW    = 100, ImgH    = 20

            Size = new Vector2(graphics.Viewport.Width / 8, graphics.Viewport.Height / 30);

        }

        public void Update(MouseState mouse)
        {
            rectangle = new Rectangle(
                (int)position.X, (int)position.Y,
                (int)Size.X, (int)Size.Y);

            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (mouseRectangle.Intersects(rectangle))
            {
                if (colour.A == 255) down = false;
                if (colour.A == 0) down = true;
                if (down) colour.A += 3; else colour.A -= 3;
                if (mouse.LeftButton == ButtonState.Pressed) isClicked = true;

            }
            else if (colour.A < 255)
            {
                colour.A += 3;
                isClicked = false;
            }
        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, rectangle, colour);
        }
    }
}
