using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AIProject
{
    class Player
    {
        public Texture2D PlayerTexture;
        public Vector2 Position;

        public bool Active;

        public float Heading;

        public int Width
        {
            get { return PlayerTexture.Width; }
        }

        public int Height
        {
            get { return PlayerTexture.Height; }
        }

        public void Initialize(Texture2D texture, Vector2 position)
        {
            PlayerTexture = texture;

            Position = position;

            Active = true;

            Heading = 0.0f;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 player_origin)
        {
            spriteBatch.Draw(PlayerTexture, Position, null, Color.White, this.Heading, player_origin, 1f, SpriteEffects.None, 0f);
        }
    }
}

