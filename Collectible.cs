using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace doubles_run
{
    public class Collectible
    {
        public Texture2D Texture;
        public Vector2 Position;
        public bool IsActive = true;
        
        public Rectangle Hitbox 
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, 40, 40);
            }
        }

        public Collectible(Texture2D texture, Vector2 startPosition)
        {
            Texture = texture;
            Position = startPosition;
        }

        public void Update(float gameSpeed)
        {
            Position.X -= 6 * gameSpeed; 
            if (Position.X < -50) 
            {
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Hitbox, Color.White);
        }
    }
}