/* Meteor.cs
 * Class for an XNA meteor
 * 
 * Revision History:
 *     Isaac Maier, 2015.11.20: Created
 *     Isaac Maier, Vivek Shyam, 2015.11.20: added animation
 *     Isaac Maier, 2015.11.22: Made speed generation based on hitpoint,
 *         stopped animation flickering, added line drawn on meteor path,
 *         fixed non-random meteor start/end points, added hackey meteor
 *         rotation so it faces in roughly where it should, changed sprite
 *         (which somehow broke the line drawing?), added FRAMENUMBER const
 *     Isaac Maier, 2015.11.24: Fixed rotation of meteors
 *     Isaac Maier, 2015.12.03: Added GetRect() method, debug draw call
 *     Isaac Maier, Vivek Shyam, 2015.12.06: Removed hitpoint from constructor,
 *         now generates hitpoint from randomX function
 */

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

//source the animation from in-class example
namespace IMVSFinalProject
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Meteor : Microsoft.Xna.Framework.GameComponent
    {
        const int FRAMENUMBER = 4;
        SpriteBatch spriteBatch;
        Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
        }

        int hitPoint;

        Vector2 speed;
        Vector2 position;
        float rotation;
        Random rand;

        public Vector2 Position
        {
            get { return position; }
        }
        
        private List<Rectangle> line;        

        //these are used for animation 
        private Vector2 dimension;

        public Vector2 Dimension
        {
            get { return dimension; }
        }
        private int frameIndex = 0;
        private int delay;
        private int delayCounter;
        private List<Rectangle> frames;

        Vector2 origin;

        public Meteor(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 screenSize)
            : base(game)
        {
            //constructor parameters
            this.spriteBatch = spriteBatch;
            this.texture = texture;

            //other fields
            rand = new Random();
            this.hitPoint = RandomX(screenSize);
            this.position = new Vector2(RandomX(screenSize), -50);

            this.speed = GenerateSpeed(screenSize);
            this.speed.Normalize();
            Vector2 speedfactor = new Vector2(6f, 6f); //TODO - tweak speed
            this.speed *= speedfactor;

            this.rotation = (float)(-Math.PI / 2 + Math.Atan2(screenSize.Y, hitPoint - position.X));
            this.origin = new Vector2(texture.Width / 8, texture.Height / 8);

            //dimension -> each frame of the animation (in the sprite sheet is 64x64)
            dimension = new Vector2(41, 92);

            delay = 5;
            delayCounter = 0;

            //set up the frames of the animations as part of the constructor (animation will be ready when called)
            createFrames();
            //createLine(screenSize);
        }

        //draws rough path of meteor, doesn't show with the new trail-4 image for some reason...
        private void createLine(Vector2 screenSize)
        {
            //add each key frame to the list of frames - by cycling through the list, we will animiate the explosion
            line = new List<Rectangle>();

            int y = 0;
            int i = 0;

            while (y < screenSize.Y)
            {
                int x = (int)(speed.X * i + position.X);
                y = (int)(speed.Y * i);
                Rectangle r = new Rectangle(x, y, 2, 2);
                line.Add(r);
                i++;
            }
        }

        private void createFrames()
        {
            //add each key frame to the list of frames - by cycling through the list, we will animiate the explosion
            frames = new List<Rectangle>();
            for (int i = 0; i < 1; i++)             //look at the sprite sheet, 1 row
            {
                for (int j = 0; j < FRAMENUMBER; j++)         //and 4 images per row...
                {
                    int x = j * (int)dimension.X;
                    int y = i * (int)dimension.Y;
                    Rectangle r = new Rectangle(x, y, (int)dimension.X, (int)dimension.Y);
                    frames.Add(r);
                }
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            position += speed;

            delayCounter++;

            if (delayCounter > delay)
            {
                frameIndex++;           //we only update the frame counter if enough time has elapsed (otherwise the animation might be too fast

                if (frameIndex > FRAMENUMBER - 1)     //if we get to 24, its time to set the count to zero (for next button click)
                {
                    frameIndex = 0;
                }

                delayCounter = 0;
            }

            base.Update(gameTime);
        }

        public void Draw()
        {
            if (frameIndex >= 0)
            {
                spriteBatch.Draw(texture, position, frames.ElementAt<Rectangle>(frameIndex), Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
                //debug
                //spriteBatch.Draw(texture, this.GetRect(), Color.Yellow);
            }
            else //this is where the blinking is coming from...
            {
                spriteBatch.Draw(texture, position, frames.ElementAt<Rectangle>(0), Color.Black, rotation, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }
        }

        //TODO - improve this
        public Rectangle GetRect()
        {
            //TODO - make this better...
            //TODO - some math here for rotation, update the Shot class, too
            //maybe multiply by rotation somehow? abs(rotation - pi/2)-ish?
            if (rotation < Math.PI / 2)
            {
                return new Rectangle((int)(position.X - dimension.X / 2 - dimension.X * (rotation - Math.PI * 0.5) - dimension.X * 1.6), (int)(position.Y - dimension.Y * 0.2),
                (int)dimension.X, (int)dimension.Y);
            }
            else if (rotation > Math.PI)
            {
                return new Rectangle((int)(position.X - dimension.X / 2 - dimension.X * (rotation - Math.PI * 0.5) - dimension.X * 0.8), (int)(position.Y - dimension.Y * 0),
                (int)dimension.X, (int)dimension.Y);
            }
            else
            {
                return new Rectangle((int)(position.X - dimension.X / 2 - dimension.X * (rotation - Math.PI * 0.5) - dimension.X * 1.3), (int)(position.Y - dimension.Y * 0.1),
                (int)dimension.X, (int)dimension.Y);
            }
        }

        public int RandomX (Vector2 screenSize)
        {
            return rand.Next(0, (int)screenSize.X + 1);
        }

        public Vector2 GenerateSpeed(Vector2 screenSize)
        {
            Random rand = new Random();

            int startX = (int)position.X;
            //int endX = RandomX(screenSize);

            Vector2 speed = new Vector2(hitPoint - startX, screenSize.Y);

            int scale = rand.Next(5, 10);

            float smallerScale = (float)scale / 1000;

            speed *= smallerScale;

            return speed;
        }
    }
}
