using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AIProject
{
    class Wall
    {
        public Texture2D WallTexture;
        public Vector2 Position;

        public bool Active;

        public int Width
        {
            get { return WallTexture.Width; }
        }

        public int Height
        {
            get { return WallTexture.Height; }
        }

        public void Initialize(Texture2D texture, Vector2 position)
        {
            WallTexture = texture;

            Position = position;

            Active = true;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 origin)
        {
            spriteBatch.Draw(WallTexture, Position, null, Color.White, 0.0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
