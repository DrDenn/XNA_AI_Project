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
    //Main Game Class
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Keyboard
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        //Graphics
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //Entities
        Player player;
        List<Enemy> enemies;
        List<Wall> walls;
        //Hud 
        SpriteFont hudTextFont;
        int sensorSelector;

        //Other
        Boolean debug; // Debug Flag
        float player_move_speed;
        float player_turn_speed;
        private Vector2 player_origin;
        private Vector2 enemy_origin;

        //What does this even do?
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        //Game Initialize
        protected override void Initialize()
        {
            //Player
            player = new Player();
            //Enemies
            enemies = new List<Enemy>();
            for (int i = 0; i < 3; i++)
            {
                enemies.Add(new Enemy());
            }
            //Walls
            walls = new List<Wall>();
            for (int i = 0; i < 2; i++)
            {
                walls.Add(new Wall());
            }

            player_move_speed = 6.0f;
            player_turn_speed = 0.05f;

            hudTextFont = Content.Load<SpriteFont>("hudTextFont");
            sensorSelector = -1;

            base.Initialize();
        }

        private Texture2D SpriteTexture_1;
        private Texture2D SpriteTexture_2;
        private Texture2D SpriteTexture_3;
        private Texture2D SpriteTexture_4;

        //Load Content
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

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

            //Walls
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
                //Add Bounding Box to player's wall list
                player.walls.Add(walls.ElementAt(i).Bounds);
            }
        }

        //Unload Content
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //Update
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
            if (previousKeyboardState.IsKeyUp(Keys.Space))
            {
                if (currentKeyboardState.IsKeyDown(Keys.Space))
                {
                    sensorSelector = (sensorSelector + 1) % 3;
                }
            }

            //Debug Check
            if (previousKeyboardState.IsKeyUp(Keys.OemTilde))
            {
                if (currentKeyboardState.IsKeyDown(Keys.OemTilde))
                {
                    debug = !debug;
                }
            }
            //Update the player
            UpdatePlayer(gameTime);

            base.Update(gameTime);
        }

        private Boolean collide = false;

        //Player Update
        private void UpdatePlayer(GameTime gameTime)
        {
            Vector2 oldP = player.Position;

            Vector2 move_direction;

            move_direction.X = (float) Math.Cos(player.Heading);
            move_direction.Y = (float) Math.Sin(player.Heading);
            
            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                player.Heading -= player_turn_speed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                player.Heading += player_turn_speed;
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

        //Collision Update
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

        //Draw
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Backdrop
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            //Start SpriteBatch
            spriteBatch.Begin();

            //Draw Player
            player.Draw(spriteBatch, player_origin);

            //Draw Enemies
            for (int i = 0; i < enemies.Count; i++)
                enemies.ElementAt(i).Draw(spriteBatch, enemy_origin);
            //Draw Walls
            for (int i = 0; i < walls.Count; i++)
                walls.ElementAt(i).Draw(spriteBatch);

            //Draw text
            StringBuilder hud = new StringBuilder();
            //First Line "Player Pos: (###, ###) Player Heading (###) "
            hud.Append("Player Pos: (" + (int)player.Position.X + ", " + (int)player.Position.Y + ") ");
            hud.Append("Player Heading: (" + (int)MathHelper.ToDegrees(player.Heading) + ") ");
            //Subsequent Lines (Note, Sensor type is on first line, everything else starts a new line
            switch (sensorSelector)
            {
                case 0: // "Wall Sensor: (###.####)..."
                    hud.AppendLine("Wall Sensor: Range (" + player.rayMax + ")");
                    for (int i = 0; i < player.rayDist.Length; i++)
                    {
                        if (player.rayDist[i] != null)
                            hud.AppendLine("(" + player.rayDist[i] + ") ");
                        else
                            hud.AppendLine("()"); // Null case
                    }
                    break;
                case 1:// "Agent Sensor: " // "Enemy #: (Distance: ### Heading: ###)" //...
                    hud.AppendLine("Agent Sensor: Range (" + player.agentMax + ")");
                    for (int i = 0; i < player.enemyData.Count(); i++)
                    {
                        if(player.enemyData.ElementAt(i).distance != -1)
                            hud.AppendLine("Enemy " + i + ": (Distance: " + player.enemyData.ElementAt(i).distance + " | Heading: (" + (int)MathHelper.ToDegrees(player.enemyData.ElementAt(i).heading) + " deg, " + player.enemyData.ElementAt(i).heading + " rad)");
                        else
                            hud.AppendLine("Enemy " + i + ": Out of Range");
                    }
                    break;
                case 2: // "Pie-Slice Sensor: " // "(#) ..."
                    hud.AppendLine("Pie-Slice Sensor: (" + player.pieMax + ")");
                    for (int i = 0; i < player.quadrants.Length; i++)
                    {
                        hud.Append("(" + player.quadrants[i] + ") ");
                    }
                    break;
                default: //"Press Spacebar to cycle through sensors!!"
                    hud.AppendLine();
                    hud.AppendLine("Press Spacebar to cycle through sensors!!");
                    break;
            }
            //Draw HUD
            spriteBatch.DrawString(hudTextFont, hud, new Vector2(0, 0), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            
            //Debug
            if(debug)
            {

                StringBuilder debugHud = new StringBuilder();
                debugHud.AppendLine("Debug: ");
                //Display 
                for (int i = 0; i < player.rayList.Length; i++)
                {
                    //Headings
                    Vector2 tempVect = new Vector2(1, 0);
                    double dot = player.rayList[i].X * tempVect.X + player.rayList[i].Y * tempVect.Y;
                    double cross = player.rayList[i].X * tempVect.Y - player.rayList[i].Y * tempVect.X;
                    if (cross >= 0) // Left
                        debugHud.Append("(" + (int)MathHelper.ToDegrees((float)Math.Acos(dot)) + ") ");
                    else
                        debugHud.Append("(" + (int)MathHelper.ToDegrees((float)((2 * Math.PI) - Math.Acos(dot))) + ") ");
                    //(X, Y)
                    //debugHud.Append("(" + (player.rayList[i].X) + ", "
                    //                    + (player.rayList[i].Y) + ") ");
                    int perLine = 4;
                    if (i % perLine == perLine-1)
                        debugHud.AppendLine();
                }
                spriteBatch.DrawString(hudTextFont, debugHud, new Vector2(0, GraphicsDevice.Viewport.Height - 100), Color.Black, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }
            //Finish SpriteBatch
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
