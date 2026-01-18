using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace doubles_run
{
    public class Character
    {
        public Texture2D Texture;      
        public Texture2D JumpTexture;  
        public Texture2D DuckTexture;  
        public Vector2 Position;
        
        public int JumpHeight = 100;
        public int JumpWidth = 90;
        public int DuckHeight = 60; 
        public int DuckWidth = 80;  
        
        public const int NORMAL_HEIGHT = 120;
        public const int WIDTH = 100;

        int currentFrame = 0;
        double frameTimer = 0;
        double fps = 12;

        public bool IsJumping = false;
        public bool IsDucking = false;
        float verticalVelocity = 0;
        float groundY;

        public int HP = 100;
        public int MaxHP = 100;
        public bool IsStunned = false;
        public float StunTimer = 0;

        // Hitbox 
        public Rectangle Hitbox 
        {
            get 
            {
                int w = WIDTH;
                int h = NORMAL_HEIGHT;

                if (IsJumping) 
                { 
                    w = JumpWidth; 
                    h = JumpHeight; 
                }
                else if (IsDucking) 
                { 
                    w = DuckWidth; 
                    h = DuckHeight; 
                }
                
                // Y fix
                int drawY = (int)Position.Y + (NORMAL_HEIGHT - h);
                return new Rectangle((int)Position.X, drawY, w, h);
            }
        }

        public Character(Texture2D texture, Vector2 startPosition)
        {
            Texture = texture;
            Position = startPosition;
            groundY = startPosition.Y; 
        }

        public void Update(GameTime gameTime)
        {
            // Stun effect
            if (IsStunned)
            {
                StunTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (StunTimer <= 0) 
                {
                    IsStunned = false;
                }
            }

            // anim speed
            int activeFrameCount = 8;
            if (IsJumping || IsDucking)
            {
                activeFrameCount = 1;
            }

            frameTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (frameTimer >= 1.0 / fps)
            {
                currentFrame = (currentFrame + 1) % activeFrameCount;
                frameTimer = 0;
            }

            // gravity
            if (Position.Y < groundY || verticalVelocity < 0)
            {
                Position.Y += verticalVelocity;
                
                float gravity = 0.5f; // fall faster if ducking in air

                if (IsDucking && IsJumping)
                {
                    gravity = 2.5f; //jump and ducking gravity
                }

                verticalVelocity += gravity;
            }
            else
            {
                Position.Y = groundY;
                IsJumping = false;
                verticalVelocity = 0;
            }
        }

        public void Jump() 
        { 
            if (!IsJumping && !IsStunned) 
            { 
                IsJumping = true; 
                verticalVelocity = -15f; //jump height
            } 
        }
        
        public void Duck(bool duckingState) 
        { 
            if (!IsStunned) 
            {
                IsDucking = duckingState; 
            }
        }
        
        public void TakeDamage(int damage) 
        { 
            HP -= damage; 
            if (HP < 0) HP = 0; 
            IsStunned = true; 
            StunTimer = 0.5f; 
        }
        
        public void Heal(int amount) 
        { 
            HP += amount; 
            if (HP > MaxHP) HP = MaxHP; 
        }

        public void Draw(SpriteBatch spriteBatch, Color defaultColor)
        {
            Texture2D activeTex = Texture;
            int sourceW = 120; 
            int sourceH = 120; 
            int drawW = WIDTH; 
            int drawH = NORMAL_HEIGHT;

            if (IsJumping) 
            { 
                activeTex = JumpTexture; 
                if (activeTex != null)
                {
                    sourceH = activeTex.Height;
                }
                else
                {
                    sourceH = 120;
                }
                
                drawW = JumpWidth; 
                drawH = JumpHeight;
            }
            else if (IsDucking) 
            { 
                activeTex = DuckTexture; 
                if (activeTex != null)
                {
                    sourceH = activeTex.Height;
                }
                else
                {
                    sourceH = 120;
                }

                drawW = DuckWidth; 
                drawH = DuckHeight;
            }

            if (activeTex == null) return;
            
            Color color = defaultColor;
            if (IsStunned)
            {
                color = Color.Red;
            }

            Rectangle sourceRect = new Rectangle(currentFrame * sourceW, 0, sourceW, sourceH);
            
            int drawY = (int)Position.Y + (NORMAL_HEIGHT - drawH);
            spriteBatch.Draw(activeTex, new Rectangle((int)Position.X, drawY, drawW, drawH), sourceRect, color);
        }
    }
}