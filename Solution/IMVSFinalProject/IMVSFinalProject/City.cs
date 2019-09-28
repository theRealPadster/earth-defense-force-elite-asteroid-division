/* City.cs
 * Class for an XNA city
 * 
 * Revision History:
 *     Isaac Maier, 2015.11.30: Created
 *     Isaac Maier, 2015.12.02: Added 3 hit max before destroyed,
 *         added GotHit(), IsDestroyed() methods,
 *         added different images for each hit state
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
    public class City : Microsoft.Xna.Framework.GameComponent
    {
        const int MAX_HITS = 3;
        const int FRAMENUMBER = 3;

        SpriteBatch spriteBatch;
        Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
        }
        int xPosition;
        Vector2 screenSize;

        Vector2 position;
        int hits;

        public int Hits
        {
            get { return hits; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        //these are used for animation 
        private Vector2 dimension;
        private int frameIndex = 0;
        private List<Rectangle> frames;

        public City(Game game, SpriteBatch spriteBatch, Texture2D texture, int xPosition, Vector2 screenSize)
            : base(game)
        {
            //constructor parameters
            this.spriteBatch = spriteBatch;
            this.texture = texture;
            this.xPosition = xPosition;
            this.screenSize = screenSize;

            //other fields
            this.position = new Vector2(xPosition, screenSize.Y - texture.Height);
            this.hits = 0;

            //dimension -> each frame of the animation (in the sprite sheet is 64x64)
            dimension = new Vector2(172, 126);

            //set up the frames of the animations as part of the constructor (animation will be ready when called)
            createFrames();
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
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public void Draw()
        {
            spriteBatch.Draw(texture, position, frames.ElementAt<Rectangle>(frameIndex), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture, this.GetRect(), Color.Red);
        }

        public Rectangle GetRect()
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width / FRAMENUMBER, texture.Height / FRAMENUMBER);
        }

        public void GotHit()
        {
            if (hits < MAX_HITS)
            {
                hits++;
                frameIndex++;
            }
        }

        public bool IsDestroyed()
        {
            if (hits == MAX_HITS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
