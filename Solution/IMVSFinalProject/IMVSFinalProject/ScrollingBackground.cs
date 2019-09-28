/* Game1.cs
 * Class for an XNA scrolling background
 * 
 * Revision History:
 *     Vivek Shyam, 2015.11.26: Created
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
    public class ScrollingBackground : Microsoft.Xna.Framework.GameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D backGroundTexture;
        private Rectangle backGroundRectangle;
        private Vector2 position, positionOne, positionTwo;
        private Vector2 speed;

        public ScrollingBackground(Game game, SpriteBatch spriteBatch, Texture2D backGroundTexture,
            Rectangle backGroundRectangle, Vector2 position, Vector2 speed)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.backGroundTexture = backGroundTexture;
            this.backGroundRectangle = backGroundRectangle;
            this.position = position;
            this.speed = speed;

            positionOne = position;
            positionTwo = new Vector2(position.X + backGroundTexture.Width, position.Y);
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

            if (positionOne.X > -backGroundTexture.Width)
            {
                positionOne.X -= speed.X;
            }
            else
            {
                positionOne.X = positionTwo.X + backGroundTexture.Width - speed.X;
            }

            if (positionTwo.X > -backGroundTexture.Width)
            {
                positionTwo.X -= speed.X;
            }
            else
            {
                positionTwo.X = positionOne.X + backGroundTexture.Width - speed.X * 2;
            }


            base.Update(gameTime);
        }

        public void Draw()
        {
            spriteBatch.Draw(backGroundTexture, positionOne, backGroundRectangle, Color.White);
            spriteBatch.Draw(backGroundTexture, positionTwo, backGroundRectangle, Color.White);
        }
    }
}
