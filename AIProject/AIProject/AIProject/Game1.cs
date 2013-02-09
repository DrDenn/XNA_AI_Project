using System;
using System.Collections.Generic;
using System.Linq;
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
        Enemy enemy_1;
        Enemy enemy_2;

        Wall wall_1;
        Wall wall_2;

        float player_move_speed;

        private Vector2 player_origin;
        private Vector2 wall_origin;

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
            // TODO: Add your initialization logic here
            player = new Player();

            enemy_1 = new Enemy();
            enemy_2 = new Enemy();

            wall_1 = new Wall();
            wall_2 = new Wall();

            player_move_speed = 6.0f;

            base.Initialize();
        }

        private Texture2D SpriteTexture_1;
        private Texture2D SpriteTexture_2;

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

            Vector2 enemyPosition1 = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 50, GraphicsDevice.Viewport.TitleSafeArea.Y +100);
            enemy_1.Initialize(Content.Load<Texture2D>("enemy_agent"), enemyPosition1);

            Vector2 enemyPosition2 = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 60, GraphicsDevice.Viewport.TitleSafeArea.Y + 200);
            enemy_2.Initialize(Content.Load<Texture2D>("enemy_agent"), enemyPosition2);

            SpriteTexture_2 = Content.Load<Texture2D>("wall_texture");

            wall_origin.X = SpriteTexture_2.Width / 2;
            wall_origin.Y = SpriteTexture_2.Height / 2;

            Vector2 wallPosition1 = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 700, GraphicsDevice.Viewport.TitleSafeArea.Y + 200);
            wall_1.Initialize(SpriteTexture_2, wallPosition1);

            Vector2 wallPosition2 = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 400, GraphicsDevice.Viewport.TitleSafeArea.Y + 200);
            wall_2.Initialize(SpriteTexture_2, wallPosition2);
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

            //Update the player
            UpdatePlayer(gameTime);

            base.Update(gameTime);
        }


        private void UpdatePlayer(GameTime gameTime)
        {
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

            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);

            float circle = MathHelper.Pi * 2;
            player.Heading = player.Heading % circle;
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
            enemy_1.Draw(spriteBatch);
            enemy_2.Draw(spriteBatch);

            wall_1.Draw(spriteBatch, wall_origin);
            //wall_2.Draw(spriteBatch, wall_origin);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
