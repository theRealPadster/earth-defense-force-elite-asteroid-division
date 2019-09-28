/* Explosion.cs
 * Class for an animated XNA explosion
 * 
 * Revision History:
 *     Isaac Maier, 2015.11.25: Created
 *     Isaac Maier, 2015.12.06: Added capability for multiple explosion
 *         sound instances at once
 *     Isaac Maier, 2015.12.07: Set custom origin point for ground/air explosions
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


namespace IMVSFinalProject
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Explosion : Microsoft.Xna.Framework.GameComponent
    {
        const int GROUND_ROWS = 3;
        const int GROUND_COLUMNS = 5;
        const int AIR_ROWS = 4;
        const int AIR_COLUMNS = 4;
        SpriteBatch spriteBatch;
        Vector2 position;
        Vector2 origin;

        //TODO - need to get textures, content.load doesn't work...
        //...so declare in Game1.cs, and pass here, or make available...
        Texture2D texture;
        bool inAir;
        bool animationOver;

        //explosion Sound Effect
        SoundEffect explosionSoundEffect;
        bool soundPlayed = false;

        public bool AnimationOver
        {
            get { return animationOver; }
        }

        //these are used for animation 
        private Vector2 dimension;
        private int frameIndex = 0;
        private int delay;
        private int delayCounter;
        private List<Rectangle> frames;
        SoundEffectInstance asteroidExplosionInstance;
            

        public Explosion(Game game, SpriteBatch spriteBatch, Vector2 position, int height, Vector2 screenSize, SoundEffect explosionSoundEffect)
            : base(game)
        {
            //constructor parameters
            this.spriteBatch = spriteBatch;
            this.position = position;
            this.explosionSoundEffect = explosionSoundEffect;

            //create new explosion sound instance for multiple explosions at once
            this.asteroidExplosionInstance = explosionSoundEffect.CreateInstance();
            this.asteroidExplosionInstance.Volume = 0.1f;

            //base which explosion image on the Y position
            if (position.Y >= screenSize.Y - height)
            {
                this.inAir = false;
                this.texture = Game1.groundExplosion;
                //dimension -> each frame of the animation (in the sprite sheet is 64x64)
                dimension = new Vector2(96, 96);
                this.origin = new Vector2(0,0);
            }
            else
            {
                this.inAir = true;
                this.texture = Game1.airExplosion;
                //dimension -> each frame of the animation (in the sprite sheet is 64x64)
                dimension = new Vector2(128, 128);
                this.origin = new Vector2(dimension.X / 2, 0);
            }

            animationOver = false;
            delay = 7;
            delayCounter = 0;

            //set up the frames of the animations as part of the constructor (animation will be ready when called)
            createFrames();
        }

        private void createFrames()
        {
            //add each key frame to the list of frames - by cycling through the list, we will animiate the explosion
            frames = new List<Rectangle>();

            if (inAir)
            {
                for (int i = 0; i < AIR_ROWS; i++)             //look at the sprite sheet, 1 row
                {
                    for (int j = 0; j < AIR_COLUMNS; j++)         //and 4 images per row...
                    {
                        int x = j * (int)dimension.X;
                        int y = i * (int)dimension.Y;
                        Rectangle r = new Rectangle(x, y, (int)dimension.X, (int)dimension.Y);
                        frames.Add(r);
                    }
                }
            }
            else
            {
                for (int i = 0; i < GROUND_ROWS; i++)             //look at the sprite sheet, 1 row
                {
                    for (int j = 0; j < GROUND_COLUMNS; j++)         //and 4 images per row...
                    {
                        int x = j * (int)dimension.X;
                        int y = i * (int)dimension.Y;
                        Rectangle r = new Rectangle(x, y, (int)dimension.X, (int)dimension.Y);
                        frames.Add(r);
                    }
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
            if (!soundPlayed)
            {
                //explosion Play Sound
                asteroidExplosionInstance.Play();
                soundPlayed = true;
            }

            delayCounter++;

            if (delayCounter > delay)
            {
                frameIndex++;   //we only update the frame counter if enough time has elapsed (otherwise the animation might be too fast

                if (inAir)
                {
                    if (frameIndex > AIR_COLUMNS * AIR_ROWS - 1)     //if we get to 24, its time to set the count to zero (for next button click)
                    {
                        animationOver = true;
                        frameIndex = 0;
                    }
                }
                else
                {
                    if (frameIndex > GROUND_COLUMNS * GROUND_ROWS - 1)     //if we get to 24, its time to set the count to zero (for next button click)
                    {
                        animationOver = true;
                        frameIndex = 0;
                    }
                }
                
                delayCounter = 0;
            }

            base.Update(gameTime);
        }

        public void Draw()
        {
            if (frameIndex >= 0)
            {
                spriteBatch.Draw(texture, position, frames.ElementAt<Rectangle>(frameIndex), Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
