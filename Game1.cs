using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace doubles_run
{
    // Game states
    enum GameState { Menu, Playing, GameOver }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        GameState currentState = GameState.Menu;
        
        Character kid;
        Character butcher;
        
        List<Obstacle> obstacles;
        List<Cloud> clouds;
        List<Projectile> projectiles;
        List<Collectible> healthItems;

        float spawnTimer = 0;
        float cloudTimer = 0;
        float hpTimer = 0;
        float projectileCooldown = 0;

        Texture2D pixelTexture;
        Texture2D playButtonTexture;
        Texture2D quitButtonTexture;
        Texture2D cloudTexture;
        Texture2D groundObstacleTexture;
        Texture2D airObstacleTexture;
        Texture2D healthTexture;
        
        //Texture2D shieldTexture;

        Rectangle playButtonRect;
        Rectangle quitButtonRect;

        float gameSpeed = 1.0f;
        float maxDistance = 100f; 
        float distance = 100f;
        float totalTime = 60f; 
        float currentTime = 60f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 500;
        }

        protected override void Initialize()
        {
            // positions
            kid = new Character(null, new Vector2(600, 320));
            butcher = new Character(null, new Vector2(50, 320));

            base.Initialize(); 
            ResetGame();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // UI bars
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            playButtonTexture = Content.Load<Texture2D>("play_button");
            quitButtonTexture = Content.Load<Texture2D>("quit_button");
            cloudTexture = Content.Load<Texture2D>("cloud_image");
            groundObstacleTexture = Content.Load<Texture2D>("obstacle_ground");
            airObstacleTexture = Content.Load<Texture2D>("obstacle_air");
            healthTexture = Content.Load<Texture2D>("health_pack");
            //shieldTexture = Content.Load<Texture2D>("shield_pack");

            // Butcher 
            butcher.Texture = Content.Load<Texture2D>("butcher_run");
            butcher.JumpTexture = Content.Load<Texture2D>("butcher_jump");
            butcher.DuckTexture = Content.Load<Texture2D>("butcher_duck");
            butcher.JumpWidth = 100;
            butcher.JumpHeight = 100;
            butcher.DuckWidth = 90;
            butcher.DuckHeight = 60;

            // Kid 
            kid.Texture = Content.Load<Texture2D>("kid_run");
            kid.JumpTexture = Content.Load<Texture2D>("kid_jump");
            kid.DuckTexture = Content.Load<Texture2D>("kid_duck");
            kid.JumpWidth = 90;
            kid.JumpHeight = 85;
            kid.DuckWidth = 75;
            kid.DuckHeight = 50;

            playButtonRect = new Rectangle(400, 200, 200, 70);
            quitButtonRect = new Rectangle(400, 300, 200, 70);
        }

        private void ResetGame()
        {
            kid.Position = new Vector2(600, 320);
            kid.HP = 100;
            kid.IsStunned = false;
            
            butcher.Position = new Vector2(50, 320);
            butcher.IsStunned = false;
            
            obstacles = new List<Obstacle>();
            clouds = new List<Cloud>();
            projectiles = new List<Projectile>();
            healthItems = new List<Collectible>();
            
            gameSpeed = 1.0f;
            distance = 100f;
            currentTime = totalTime;
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);

            // State
            switch (currentState)
            {
                case GameState.Menu:
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (playButtonRect.Contains(mousePoint)) 
                        {
                            currentState = GameState.Playing;
                        }
                        if (quitButtonRect.Contains(mousePoint)) 
                        {
                            Exit();
                        }
                    }
                    break;

                case GameState.Playing:
                    UpdateGame(gameTime);
                    break;

                case GameState.GameOver:
                    // Restart on click or enter
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) || mouseState.LeftButton == ButtonState.Pressed)
                    {
                        ResetGame();
                        currentState = GameState.Menu;
                    }
                    break;
            }
            base.Update(gameTime);
        }

        private void UpdateGame(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentTime -= deltaTime;
            gameSpeed += 0.0005f; 

            var keyboardState = Keyboard.GetState();
            
            // Kid controls
            if (keyboardState.IsKeyDown(Keys.Up)) 
            {
                kid.Jump();
            }
            // kid.Update(gameTime); //
            kid.Duck(keyboardState.IsKeyDown(Keys.Down));
            
            // Butcher controls
            // if (kstate.IsKeyDown(Keys.Space)) kid.Position.Y -= 10; // test
            if (keyboardState.IsKeyDown(Keys.W)) 
            {
                butcher.Jump();
            }
            butcher.Duck(keyboardState.IsKeyDown(Keys.S));
            
            // Butcher 
            projectileCooldown -= deltaTime;
            if (keyboardState.IsKeyDown(Keys.Space) && projectileCooldown <= 0)
            {
                projectiles.Add(new Projectile(pixelTexture, new Vector2(butcher.Position.X + 80, butcher.Position.Y + 40)));
                projectileCooldown = 2.0f;
            }

            kid.Update(gameTime);
            butcher.Update(gameTime);

            // Butcher follows the Kid and Move
            float butcherTargetX = kid.Position.X - (distance * 4.5f);
            if (butcher.Position.X < butcherTargetX)
            {
                butcher.Position.X += 2.0f * gameSpeed; 
            }
            else if (butcher.Position.X > butcherTargetX)
            {
            butcher.Position.X -= 2.0f * gameSpeed; 
            }

            UpdateEnvironment(deltaTime);
            CheckGameOver();
        }

        private void UpdateEnvironment(float deltaTime)
        {
            // Create clouds
            cloudTimer += deltaTime;
            if (cloudTimer > 1.5f / gameSpeed)
            {
                float rndScale = (float)(new System.Random().NextDouble() + 0.5);
                clouds.Add(new Cloud(cloudTexture, new Vector2(1050, new System.Random().Next(20, 200)), rndScale * 1.5f, rndScale));
                cloudTimer = 0;
            }
            
            // Remove clouds
            for(int i = clouds.Count - 1; i >= 0; i--) 
            {
                clouds[i].Update(gameSpeed);
                if(clouds[i].Position.X < -100) 
                {
                    clouds.RemoveAt(i);
                }
            }

            // Create obstacles
            spawnTimer += deltaTime;
                if (spawnTimer > 1.8f / gameSpeed)
            {
                int type = new System.Random().Next(0, 2); 
                
                Texture2D selectedTex;
                
                int w, h, spawnY;

                if (type == 0)
                {
                    selectedTex = groundObstacleTexture;
                    w = 50; 
                    h = 100;
                    spawnY = 440 - h;
                }
                else
                {
                    selectedTex = airObstacleTexture;
                    w = 125;
                    h = 70;
                    spawnY = 280;
                }
                
                obstacles.Add(new Obstacle(selectedTex, new Vector2(1050, spawnY), w, h));
                spawnTimer = 0; 
            }

            // Update obstacles
            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                obstacles[i].Update(gameSpeed);

                if (obstacles[i].IsActive && !kid.IsStunned && kid.Hitbox.Intersects(obstacles[i].Hitbox)) // checking collision with Kid
                {
                    kid.TakeDamage(10);
                    distance -= 15;
                }
                
                if (obstacles[i].IsActive && !butcher.IsStunned && butcher.Hitbox.Intersects(obstacles[i].Hitbox)) // check collision with Butcher
                {
                    butcher.TakeDamage(10); 
                    distance += 10; 
                }

                if (!obstacles[i].IsActive) 
                {
                    obstacles.RemoveAt(i);
                }
            }

            ProcessLists(deltaTime);
        }

        private void ProcessLists(float deltaTime)
        {
            // Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(gameSpeed);
                if (projectiles[i].IsActive && projectiles[i].Hitbox.Intersects(kid.Hitbox))
                {
                    kid.TakeDamage(15);
                    projectiles[i].IsActive = false;
                }
                
                if (!projectiles[i].IsActive) 
                {
                    projectiles.RemoveAt(i);
                }
            }

            // Health packs
            hpTimer += deltaTime;
            if (hpTimer > 20.0f)
            {
                healthItems.Add(new Collectible(healthTexture, new Vector2(1050, 360)));
                hpTimer = 0;
            }

            for (int i = healthItems.Count - 1; i >= 0; i--)
            {
                healthItems[i].Update(gameSpeed);
                if (healthItems[i].IsActive && kid.Hitbox.Intersects(healthItems[i].Hitbox))
                {
                    kid.Heal(20);
                    healthItems[i].IsActive = false;
                }
                
                if (!healthItems[i].IsActive) 
                {
                    healthItems.RemoveAt(i);
                }
            }
        }

        private void CheckGameOver() //game over check
        {
            if (kid.HP <= 0 || distance <= 5)
            {
                EndGame();
            }
            else if (currentTime <= 0)
            {
                EndGame();
            }
            else if (butcher.Position.X < -50)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            currentState = GameState.GameOver;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            if (currentState == GameState.Menu)
            {
                _spriteBatch.Draw(playButtonTexture, playButtonRect, Color.White);
                _spriteBatch.Draw(quitButtonTexture, quitButtonRect, Color.White);
            }
            else if (currentState == GameState.Playing)
            {
                // Floor
                _spriteBatch.Draw(pixelTexture, new Rectangle(0, 440, 1000, 60), new Color(30, 100, 30));
                
                foreach (var c in clouds) c.Draw(_spriteBatch);
                kid.Draw(_spriteBatch, Color.Blue);
                butcher.Draw(_spriteBatch, Color.Red);
                foreach (var o in obstacles) o.Draw(_spriteBatch);
                foreach (var p in projectiles) p.Draw(_spriteBatch);
                foreach (var h in healthItems) h.Draw(_spriteBatch);
                
                DrawUI();
            }
            else if (currentState == GameState.GameOver)
            {
                 // game over screen
                 GraphicsDevice.Clear(Color.Black);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawUI()
        {
            // 1. Distance Bar (Center)
            int distBarWidth = 300;
            int distBarX = 350;
            
            float distRatio = distance / maxDistance;
            if (distRatio > 1) distRatio = 1;
            if (distRatio < 0) distRatio = 0;
            
            // Background
            _spriteBatch.Draw(pixelTexture, new Rectangle(distBarX, 20, distBarWidth, 20), Color.Gray);
            
            Color barColor = Color.Green;
            if (distRatio < 0.5f)
            {
                barColor = Color.Red;
            }

            _spriteBatch.Draw(pixelTexture, new Rectangle(distBarX, 20, (int)(distBarWidth * distRatio), 20), barColor);

            // 2. Kid Health (Right)
            float hpRatio = kid.HP / (float)kid.MaxHP;
            _spriteBatch.Draw(pixelTexture, new Rectangle(780, 20, 200, 20), Color.Black);
            _spriteBatch.Draw(pixelTexture, new Rectangle(780, 20, (int)(200 * hpRatio), 20), Color.Red);

            // 3. Time Bar (Left)
            float timeRatio = currentTime / totalTime;
            if(timeRatio < 0) timeRatio = 0;
            
            _spriteBatch.Draw(pixelTexture, new Rectangle(20, 20, 200, 20), Color.Black);
            _spriteBatch.Draw(pixelTexture, new Rectangle(20, 20, (int)(200 * timeRatio), 20), Color.Yellow);
        }
    }
}