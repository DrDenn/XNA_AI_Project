using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AIProject
{
    class Enemy
    {
        public Texture2D EnemyTexture;

        // The position of the enemy ship relative to the top left corner of thescreen
        public Vector2 Position;

        // The state of the Enemy Ship
        public bool Active;


        // Get the width of the enemy ship
        public int Width
        {
            get { return EnemyTexture.Width; }
        }

        // Get the height of the enemy ship
        public int Height
        {
            get { return EnemyTexture.Height; }
        }

        public float Heading;

        public void Initialize(Texture2D texture, Vector2 position)
        {
            EnemyTexture = texture;

            Position = position;

            Active = true;

            Heading = 0.0f;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 origin)
        {
            spriteBatch.Draw(EnemyTexture, Position, null, Color.White, 0.0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
