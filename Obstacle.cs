using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace doubles_run
{
    public class Obstacle
    {
        public Texture2D Texture;
        public Vector2 Position;
        public int Width;
        public int Height;
        public bool IsActive = true;

        public Rectangle Hitbox
        {
            get
            {
                // hitbox width
                if (Width == 125)
                {
                    int narrowWidth = 40; 
                    int offsetX = (Width - narrowWidth) / 2; 
                    return new Rectangle((int)Position.X + offsetX, (int)Position.Y, narrowWidth, Height);
                }
                
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        public Obstacle(Texture2D texture, Vector2 position, int width, int height)
        {
            Texture = texture;
            Position = position;
            Width = width;
            Height = height;
        }

        public void Update(float gameSpeed)
        {
            Position.X -= 5 * gameSpeed;
            if (Position.X < -200) 
            {
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
        }
    }
}