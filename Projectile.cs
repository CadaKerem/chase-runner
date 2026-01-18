using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace doubles_run
{
    public class Projectile
    {
        public Texture2D Texture;
        public Vector2 Position;
        public bool IsActive = true;
        
        public Rectangle Hitbox 
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, 20, 10);
            }
        }

        public Projectile(Texture2D texture, Vector2 startPosition)
        {
            Texture = texture;
            Position = startPosition;
        }

        public void Update(float speedMultiplier)
        {
            Position.X += 10 * speedMultiplier; 
            if (Position.X > 1100) 
            {
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Hitbox, Color.Black);
        }
    }
}