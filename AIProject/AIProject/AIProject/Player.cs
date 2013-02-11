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
        public List<EnemyData> enemyData;
        public float[] rayDist;
        public float[] rayHeadings;
        public List<BoundingBox> walls;
        public int[] quadrants;

        public int rayCount;
        public int rayMax;
        public int agentMax;
        public int pieMax;

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

            enemyData = new List<EnemyData>();
            walls = new List<BoundingBox>();

            rayCount = 8;
            rayDist = new float[rayCount];
            rayHeadings = new float[rayCount];

            quadrants = new int[4];


            rayMax = 300;
            agentMax = 150;
            pieMax = 150;
        }

        public void Update()
        {
            //Initialize
            for(int i = 0 ; i < rayDist.Length ; i++)
                rayDist[i] = -1;
            quadrants[0] = quadrants[1] = quadrants[2] = quadrants[3] = 0;
            Vector2 headingVector = new Vector2((float)Math.Cos(Heading), (float)Math.Sin(Heading));
            headingVector.Normalize();
            
            //Enemy Detection update
            for (int i = 0; i < enemyData.Count(); i++)
            {
                enemyData.ElementAt(i).Update(this.Position, headingVector);
                quadrants[enemyData.ElementAt(i).quadrant]++;
            }

            //Raycasting update ( _\|/_ )
            for (int i = 0; i < rayCount; i++)
            {
                //Initial
                float x = (float)(Math.Sin(MathHelper.ToRadians(45 * i)));
                float y = (float)(Math.Cos(MathHelper.ToRadians(45 * i)));

                //Based on heading
                x = (float)(x * Math.Cos(Heading) - y * Math.Sin(Heading));
                y = (float)(x * Math.Sin(Heading) + y * Math.Cos(Heading));

                Ray thisRay = new Ray(new Vector3(this.Position.X, Position.Y, 0), new Vector3(x, y, 0));
                rayHeadings[i] = (float)Math.Acos(x);

                //Wall Detection
                for (int j = 0; j < walls.Count; j++)
                {
                    BoundingBox temp = walls.ElementAt(j);
                    Nullable<Single> tempDist;
                    tempDist = thisRay.Intersects(temp);
                    if (tempDist != null)
                    {
                        if (rayDist[i] < 0 || rayDist[i] > tempDist)
                        {
                            rayDist[i] = (float)tempDist;
                        }
                    }
                    else
                    {
                        rayDist[i] = -2;
                    }
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 player_origin)
        {
            spriteBatch.Draw(PlayerTexture, Position, null, Color.White, this.Heading, player_origin, 1f, SpriteEffects.None, 0f);
        }

        public class EnemyData
        {
            public Enemy enemy;
            public float distance;
            public int heading;
            public int quadrant;

            public void Initialize(Enemy enemy)
            {
                this.enemy = enemy;
                distance = 0;
                heading = 0;
                quadrant = 0;
            }

            public void Update(Vector2 position, Vector2 playerVector)
            {
                float x = position.X - enemy.Position.X;
                float y = position.Y - enemy.Position.Y;
                Vector2 enemyVector = new Vector2(x, y);
                enemyVector.Normalize();
                
                float dot = enemyVector.X * playerVector.X + enemyVector.Y * playerVector.Y;
                float cross = playerVector.X * enemyVector.Y - enemyVector.X * playerVector.Y;

                if (cross >= 0)
                {
                    this.heading = (int)MathHelper.ToDegrees((float)Math.Acos(dot));
                }
                else
                {
                    this.heading = 360 - (int)MathHelper.ToDegrees((float)Math.Acos(dot));
                }
                this.heading = ((this.heading + 180) % 360) % 4;
                quadrant = ((int)this.heading) / 90;

                distance = (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            }
        }

    }
}

