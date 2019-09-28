/* Shot.cs
 * Class for an XNA shot
 * 
 * Revision History:
 *     Isaac Maier, 2015.11.24: Created
 *     Isaac Maier, 2015.11.25: Added animation, normalised speeds
 *     Isaac Maier, 2015.12.03: Added GetRect() method, debug draw for collisions
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
    public class Shot : Microsoft.Xna.Framework.GameComponent
    {
        const int FRAMENUMBER = 4;
        SpriteBatch spriteBatch;
        Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
        }

        Vector2 target;
        Turret turret;
        Vector2 screenSize;

        Vector2 position;
        Vector2 speed;
        Vector2 origin;
        float rotation;

        //Shot Sound Effect
        SoundEffectInstance shotSoundEffect;
        bool soundPlayed = true;

        public Vector2 Position
        {
            get { return position; }
        }

        //these are used for animation 
        private Vector2 dimension;
        private int frameIndex = 0;
        private int delay;
        private int delayCounter;
        private List<Rectangle> frames;

        public Shot(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 target, Turret turret, Vector2 screenSize, SoundEffectInstance shotSoundEffect)
            : base(game)
        {
            //constructor parameters
            this.spriteBatch = spriteBatch;
            this.texture = texture;
            this.target = target;
            this.turret = turret;
            this.screenSize = screenSize;
            this.shotSoundEffect = shotSoundEffect;

            //other fields
            this.position = turret.Position;
            origin = new Vector2(texture.Width / 8, texture.Height / 2);
            this.rotation = turret.Rotation;

            this.speed = new Vector2(target.X - turret.Position.X, -(turret.Position.Y - target.Y));
            this.speed.Normalize();
            Vector2 speedfactor = new Vector2(8f, 8f);
            this.speed *= speedfactor;
                      
            //animation
            dimension = new Vector2(19, 59);
            delay = 7;
            delayCounter = 0;

            //set up the frames of the animations as part of the constructor (animation will be ready when called)
            createFrames();
        }

        private void createFrames()
        {
            //add each key frame to the list of frames - by cycling through the list, we will animiate the explosion
            frames = new List<Rectangle>();
            //origins = new List<Vector2>();
            for (int i = 0; i < 1; i++)             //look at the sprite sheet, 1 row
            {
                for (int j = 0; j < FRAMENUMBER; j++)         //and 4 images per row...
                {
                    int x = j * (int)dimension.X;
                    int y = i * (int)dimension.Y;
                    Rectangle r = new Rectangle(x, y, (int)dimension.X, (int)dimension.Y);
                    frames.Add(r);
                    //Vector2 v = new Vector2(x + dimension.X / 2, y + dimension.Y / 2);
                    //origins.Add(v);
                }
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

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

            if (soundPlayed)
            {
                //Shot Play Sound
                shotSoundEffect.Play();
                soundPlayed = false;
            }

            if (delayCounter > delay)
            {
                frameIndex++;   //we only update the frame counter if enough time has elapsed (otherwise the animation might be too fast

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
                spriteBatch.Draw(texture, position, frames.ElementAt<Rectangle>(frameIndex), Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
                //debug
                //spriteBatch.Draw(texture, this.GetRect(), Color.Red);
            }
        }

        //TODO - this is not working 100% perfectly - tweak for when angled...
        public Rectangle GetRect()
        {
            return new Rectangle((int)(position.X - dimension.X / 2), (int)(position.Y - dimension.Y / 2), (int)dimension.X, (int)dimension.Y);
        }
    }
}
