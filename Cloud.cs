using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace doubles_run
{
    public class Cloud
    {
        public Texture2D Texture;
        public Vector2 Position;
        public float Speed;
        public float Scale;

        public Cloud(Texture2D texture, Vector2 position, float speed, float scale)
        {
            Texture = texture;
            Position = position;
            Speed = speed;
            Scale = scale;
        }

        public void Update(float gameSpeed)
        {
            // background speed
            Position.X -= Speed * gameSpeed * 0.5f; 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = (int)(100 * Scale);
            int height = (int)(50 * Scale);
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, width, height), Color.LightGray);
        }
    }
}