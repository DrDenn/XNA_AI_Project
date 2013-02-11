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
        //Player Data
        public Texture2D PlayerTexture;
        public Vector2 Position;
        public bool Active;
        public float Heading;

        //Sensor Data
        public List<EnemyData> enemyData;
        public List<BoundingBox> walls;

        public Nullable<float>[] rayDist;
        public int rayCount;
        public int[] quadrants;
        public Vector2[] rayList;

        //Sensor Limits
        public Vector2 FirstRay;
        public int rayMax;
        public int agentMax;
        public int pieMax;        
        //Get Height
        public int Width
        {
            get { return PlayerTexture.Width; }
        }
        //Get Width
        public int Height
        {
            get { return PlayerTexture.Height; }
        }
        //Initialize
        public void Initialize(Texture2D texture, Vector2 position)
        {
            //Initialize
            PlayerTexture = texture;
            Position = position;            
            Heading = 0.0f;

            //Sensor Data
            enemyData = new List<EnemyData>();
            walls = new List<BoundingBox>();

            //Number of Rays to be cast
            rayCount = 5;
            rayDist = new Nullable<float>[rayCount];
            rayList = new Vector2[rayCount];
            
            //Number of Quadrants
            quadrants = new int[4];

            //Max Values
            FirstRay = new Vector2(0, -1);
            //FirstRay.Normalize();

            rayMax = 250;
            agentMax = 500;
            pieMax = 150;

            //ACTIVATE
            Active = true;
        }
        //Update
        public void Update()
        {
            //Initialize
            //Set Ray Distances to -1 each update 
            for(int i = 0 ; i < rayDist.Length ; i++)
                rayDist[i] = null;
            //Set Quadrant counts to -1 each update
            for (int i = 0; i < quadrants.Length; i++)
                quadrants[i] = 0;
            //Build Player Heading Vector and normalize
            Vector2 headingVector = new Vector2((float)Math.Cos(Heading), (float)Math.Sin(Heading));
            headingVector.Normalize();
            
            //Enemy Detection update
            for (int i = 0; i < enemyData.Count(); i++)
            {
                enemyData.ElementAt(i).Update(this.Position, headingVector, pieMax, agentMax);
                if(enemyData.ElementAt(i).quadrant != -1)
                 quadrants[enemyData.ElementAt(i).quadrant]++;
            }

            //Raycasting update ( _\|/_ )
            for (int i = 0; i < rayCount; i++)
            {
                double iTheta = (0.25f * Math.PI) * i;
                float x = (float)(FirstRay.X * Math.Cos(iTheta) - FirstRay.Y * Math.Sin(iTheta));
                float y = (float)(FirstRay.X * Math.Sin(iTheta) + FirstRay.Y * Math.Cos(iTheta));
                double e = 0.00000001;
                if (x < e && x > -e)
                    x = 0;
                if (y < e && y > -e)
                    y = 0;
                //Based on heading
                float newX = (float)(x * Math.Cos(Heading) - y * Math.Sin(Heading));
                float newY = (float)(x * Math.Sin(Heading) + y * Math.Cos(Heading));

                Vector2 newRay = new Vector2(newX, newY);
                newRay.Normalize();

                //Generate Ray
                Ray ray = new Ray(new Vector3(Position.X, Position.Y, 0f),
                                  new Vector3(newRay.X, newRay.Y, 0f));
                rayList[i] = new Vector2(ray.Direction.X, ray.Direction.Y);
                //Wall Detection
                for (int j = 0; j < walls.Count; j++)
                {
                    Nullable<float> tempDist = ray.Intersects(walls.ElementAt(j));
                    if (tempDist <= rayMax)
                    {
                        if (rayDist[i] < 0 || rayDist[i] == null || rayDist[i] > tempDist)
                        {
                            rayDist[i] = tempDist;
                        }
                    }
                }
            }
        }
        //Draw
        public void Draw(SpriteBatch spriteBatch, Vector2 player_origin)
        {
            spriteBatch.Draw(PlayerTexture, Position, null, Color.White, this.Heading, player_origin, 1f, SpriteEffects.None, 0f);
        }

        //EnemyData class
        public class EnemyData
        {
            public Enemy enemy;
            public float distance;
            public float heading; //Radians
            public int quadrant;
            public Boolean outOfrangeAgent;
            public Boolean outOfrangePie;

            public void Initialize(Enemy enemy)
            {
                this.enemy = enemy;
                distance = -1;
                heading  = -1;
                quadrant = -1;
            }
            //Update
            public void Update(Vector2 position, Vector2 playerVector, int pieMax, int agentMax)
            {
                outOfrangePie = outOfrangeAgent = false;
                //Initilize x, y, and enemy Direction Vector
                float x = position.X - enemy.Position.X;
                float y = position.Y - enemy.Position.Y;
                Vector2 enemyVector = new Vector2(x, y);

                //Set Distance
                distance = enemyVector.Length();
                if (distance > agentMax)
                    outOfrangeAgent = true;
                if (distance > pieMax)
                    outOfrangePie = true;


                //Normalize Enemy Direction Vector and Caculate products
                enemyVector.Normalize();
                float dot = enemyVector.X * playerVector.X + enemyVector.Y * playerVector.Y;
                float cross = playerVector.X * enemyVector.Y - enemyVector.X * playerVector.Y;

                //Set Heading and Quadrant
                if (!outOfrangePie)
                {
                    if (cross >= 0) // Left
                    {
                        //Set Heading
                        heading = (float)((2 * Math.PI) - Math.Acos(dot));
                        if (dot >= 0)
                            quadrant = 2; // Left Behind Quadrant
                        else
                            quadrant = 1; // Left Ahead Quadrant
                    }
                    else // Right
                    {
                        //Set Heading
                        heading = (float)Math.Acos(dot);
                        if (dot >= 0)
                            quadrant = 3; // Right Behind Quadrant
                        else
                            quadrant = 0; // Right Ahead Quadrant
                    }
                    heading = (float)((heading + Math.PI) % (2 * Math.PI));
                }
                else
                {
                    distance = -1;
                    heading  = -1;
                    quadrant = -1;
                }
            }
        }

    }
}

