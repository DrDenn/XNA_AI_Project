using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AIProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player;
        List<Enemy> enemies;
        List<Wall> walls;

        //Hud Display
        SpriteFont hudFont;
        int sensorSelector;

        Boolean sensorChange;

        float player_move_speed;

        private Vector2 player_origin;
        private Vector2 enemy_origin;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            player = new Player();

            enemies = new List<Enemy>();
            for (int i = 0; i < 3; i++)
            {
                enemies.Add(new Enemy());
            }
            walls = new List<Wall>();
            for (int i = 0; i < 1; i++)
            {
                walls.Add(new Wall());
            }

            player_move_speed = 6.0f;

            hudFont = Content.Load<SpriteFont>("hudFont");
            sensorSelector = -1;
            sensorChange = false;

            base.Initialize();
        }

        private Texture2D SpriteTexture_1;
        private Texture2D SpriteTexture_2;
        private Texture2D SpriteTexture_3;
        private Texture2D SpriteTexture_4;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Load the player resources 
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width / 2, 
                GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);    

            SpriteTexture_1 = Content.Load<Texture2D>("player");

            player.Initialize(SpriteTexture_1, playerPosition);

            player_origin.X = SpriteTexture_1.Width / 2;
            player_origin.Y = SpriteTexture_1.Height / 2;

            SpriteTexture_2 = Content.Load<Texture2D>("enemy_agent");

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies.ElementAt(i).Initialize(SpriteTexture_2, new Vector2(300 + 50 * i, 300 + 75 * i));
                player.enemyData.Add(new Player.EnemyData());
                player.enemyData.ElementAt(i).Initialize(enemies.ElementAt(i));
            }

            enemy_origin.X = SpriteTexture_2.Width / 2;
            enemy_origin.Y = SpriteTexture_2.Height / 2;

            SpriteTexture_3 = Content.Load<Texture2D>("wall_1");
            SpriteTexture_4 = Content.Load<Texture2D>("wall_2");

            for (int i = 0; i < walls.Count; i++)
            {
                if (i % 2 == 0)
                {
                    walls.ElementAt(i).Initialize(SpriteTexture_3, new Vector2(25 + 50 * i, 25 + 50 * i));
                    
                }
                else
                {
                    walls.ElementAt(i).Initialize(SpriteTexture_4, new Vector2(50 + 50 * i, 50 + 50 * i));

                }
                Wall temp = walls.ElementAt(i);
                player.walls.Add(new BoundingBox(new Vector3(temp.Position.X, temp.Position.Y, -1), new Vector3(temp.Position.X + temp.Width+100, temp.Position.Y + temp.Height+100, 1)));
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();

            //Cycle through sensor types
            if (!sensorChange)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Space))
                {
                    sensorSelector = (sensorSelector + 1) % 3;
                    sensorChange = true;
                }
            }
            else
            {
                if (currentKeyboardState.IsKeyUp(Keys.Space))
                {
                    sensorChange = false;
                }
            }

                //Update the player
                UpdatePlayer(gameTime);

            base.Update(gameTime);
        }

        private Boolean collide = false;

        private void UpdatePlayer(GameTime gameTime)
        {
            Vector2 oldP = player.Position;

            Vector2 move_direction;

            move_direction.X = (float) Math.Cos(player.Heading);
            move_direction.Y = (float) Math.Sin(player.Heading);
            
            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                player.Heading -= 0.05f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                player.Heading += 0.05f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                player.Position += (move_direction * player_move_speed);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                player.Position -= (move_direction * player_move_speed);
            }


            UpdateCollision();
            if (collide)
            {
                player.Position = oldP;
            }

            // Make sure that the player does not go out of bounds
            float temp = (float)Math.Sqrt(Math.Pow(player.Width, 2) + Math.Pow(player.Height, 2)) / 2;
            player.Position.X = MathHelper.Clamp(player.Position.X, 0 + temp, GraphicsDevice.Viewport.Width - temp);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0 + temp, GraphicsDevice.Viewport.Height - temp);

            float circle = MathHelper.Pi * 2;
            player.Heading = player.Heading % circle;
            if (player.Heading < 0)
                player.Heading += circle;

            player.Update();
        }


        private void UpdateCollision()
        {
            //Player Rectangle
            Rectangle playerRect = new Rectangle((int)player.Position.X - (player.Width / 2), (int)player.Position.Y - (player.Height / 2), player.Width, player.Height);
            Rectangle check;
            collide = false;
            for (int i = 0; i < enemies.Count ; i++)
            {
                Enemy temp = enemies.ElementAt(i);
                check = new Rectangle((int)temp.Position.X - (temp.Width / 2), (int)temp.Position.Y - (temp.Height / 2), temp.Width, temp.Height);
                if(playerRect.Intersects(check))
                {
                    collide = true;
                }
            }
            for (int i = 0; i < walls.Count ; i++)
            {
                Wall temp = walls.ElementAt(i);
                check = new Rectangle((int)temp.Position.X, (int)temp.Position.Y, temp.Width, temp.Height);
                if(playerRect.Intersects(check))
                {
                    collide = true;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();


            player.Draw(spriteBatch, player_origin);

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies.ElementAt(i).Draw(spriteBatch, enemy_origin);
            }
            for (int i = 0; i < walls.Count; i++)
            {
                walls.ElementAt(i).Draw(spriteBatch);
            }

            //Draw text
            spriteBatch.DrawString(hudFont, "Player Pos: " + (int)player.Position.X + " x, " + (int)player.Position.Y + " y, Player Heading: " + (int)MathHelper.ToDegrees(player.Heading),
                                   new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White) ;
            /*
            StringBuilder temp = new StringBuilder();
            for (int i = 0; i < player.rayHeadings.Length; i++)
            {
                temp.Append((int)MathHelper.ToDegrees(player.rayHeadings[i]) + " ");
            }
            spriteBatch.DrawString(hudFont, temp.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            */
            StringBuilder sensorInfo = new StringBuilder();
            switch (sensorSelector)
            {
                case -1:
                    sensorInfo.Append("Press Spacebar to cycle through sensors!!");
                    break;
                case 0:
                    sensorInfo.AppendLine("Wall Sensor: ");
                    for (int i = 0; i < player.rayDist.Length; i++)
                    {
                        if (player.rayDist[i] != -1)
                            sensorInfo.AppendLine("(" + player.rayDist[i] + ") ");
                        else
                            sensorInfo.AppendLine("( )");
                    }
                    break;
                case 1:
                    sensorInfo.AppendLine("Agent Sensor: ");
                    for (int i = 0; i < player.enemyData.Count(); i++)
                    {
                        sensorInfo.AppendLine("Enemy " + i + ": (Distance: " + player.enemyData.ElementAt(i).distance + " Heading: " + player.enemyData.ElementAt(i).heading + ")");
                    }
                    break;
                case 2:
                    sensorInfo.AppendLine("Pie-Slice Sensor: ");
                    for (int i = 0; i < player.quadrants.Length; i++)
                    {
                        sensorInfo.Append("(" + player.quadrants[i] + ") ");
                    }
                    break;
            }
            spriteBatch.DrawString(hudFont, sensorInfo.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 50), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
