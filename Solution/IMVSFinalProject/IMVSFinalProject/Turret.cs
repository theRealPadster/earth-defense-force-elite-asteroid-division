/* Turret.cs
 * Class for an XNA turret
 * 
 * Revision History:
 *     Isaac Maier, 2015.11.24: Created
 *     Isaac Maier, 2015.11.25: Made turret rotate to face mouse in real time
 *     Isaac Maier, 2015.11.26: Added animation when gun shoots
 *     Isaac Maier, 2015.12.09: Added turret mount
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
    public class Turret : Microsoft.Xna.Framework.GameComponent
    {
        int FRAMENUMBER = 5;
        SpriteBatch spriteBatch;
        Texture2D texture;
        Texture2D mountTexture;
        Vector2 mountPosition;
        Vector2 screenSize;

        Vector2 position;
        float rotation;
        bool isClicked;
        bool displayAnim;

        public bool DisplayAnim
        {
            get { return displayAnim; }
            set { displayAnim = value; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public float Rotation
        {
            get { return rotation; }
        }

        public bool IsClicked
        {
            get { return isClicked; }
        }

        //these are used for animation 
        private Vector2 dimension;
        private int frameIndex = 0;
        private int delay;
        private int delayCounter;
        private List<Rectangle> frames;

        public Turret(Game game, SpriteBatch spriteBatch, Texture2D texture, Texture2D mountTexture, Vector2 screenSize)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.texture = texture;
            this.mountTexture = mountTexture;
            this.screenSize = screenSize;

            position = new Vector2(screenSize.X / 2, screenSize.Y - texture.Height / 2);
            mountPosition = new Vector2(position.X - 25, position.Y);
            rotation = 0f;
            isClicked = false;
            displayAnim = false;

            dimension = new Vector2(64, 104);
            delay = 3;
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
        public void Update(GameTime gameTime, MouseState mouseState, MouseState previousMouseState)
        {
            //TODO - make so you can't click off, drag on, and activate button
            isClicked = false;

            Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

            int xDiff = (int)(mousePos.X - this.position.X);
            int yDiff = (int)(mousePos.Y - this.position.Y);

            rotation = (float)(Math.Atan2(yDiff, xDiff) + Math.PI / 2);

            if (previousMouseState.LeftButton == ButtonState.Pressed &&
                mouseState.LeftButton == ButtonState.Released)
            {
                isClicked = true;
                //reset animation whenever you click
                frameIndex = 0;
            }

            //to finish an animation if partially through when bool gets swapped
            if (displayAnim || frameIndex != 0)
            {
                delayCounter++;

                if (delayCounter > delay)
                {
                    frameIndex++;   //we only update the frame counter if enough time has elapsed (otherwise the animation might be too fast

                    if (frameIndex > FRAMENUMBER - 1)     //if we get to 24, its time to set the count to zero (for next button click)
                    {
                        displayAnim = false;
                        frameIndex = 0;
                    }

                    delayCounter = 0;
                }
            }

            base.Update(gameTime);
        }

        public void Draw()
        {
            if (frameIndex >= 0)
            {
                spriteBatch.Draw(mountTexture, mountPosition, Color.White);
                spriteBatch.Draw(texture, position, frames.ElementAt<Rectangle>(frameIndex), Color.White, rotation, new Vector2(texture.Width / 12, texture.Height / 2), 1f, SpriteEffects.None, 0);
            }
        }
    }
}
