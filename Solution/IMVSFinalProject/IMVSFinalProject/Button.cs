/* Button.cs
 * Class for an XNA button
 * 
 * Revision History:
 *     Isaac Maier, 2015.11.19: Created
 *     Isaac Maier, 2015.11.19: Added clicking functionality
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
    public class Button : Microsoft.Xna.Framework.GameComponent
    {
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Texture2D texture;
        string message;
        Vector2 position;
        Color colour;
        
        int padding;
        Rectangle buttonRect;
        bool isClicked;

        public Color Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        public bool IsClicked
        {
            get { return isClicked; }
        }
        
        
        public Button(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, Texture2D texture, string message, Vector2 position, Color colour)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
            this.texture = texture;
            this.message = message;
            this.position = position;
            this.colour = colour;

            padding = 5;
            this.buttonRect = new Rectangle((int)position.X - padding, (int)position.Y, (int)spriteFont.MeasureString(message).X + 2 * padding, (int)spriteFont.MeasureString(message).Y);
            this.isClicked = false;
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

        //NOTE - CITE - added some code inspired and adapted from
        //http://www.spikie.be/blog/page/Building-a-main-menu-and-loading-screens-in-XNA-Page-3.aspx
        public void Update(GameTime gameTime, MouseState mouseState, MouseState previousMouseState)
        {
            isClicked = false;

            //TODO - make so you can't click off, drag on, and activate button

            if (previousMouseState.LeftButton == ButtonState.Pressed && 
                mouseState.LeftButton == ButtonState.Released)
            {
                Rectangle mousePos = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

                if (mousePos.Intersects(buttonRect))
                {
                    isClicked = true;
                }
            }

            base.Update(gameTime);
        }

        public void Draw()
        {
            spriteBatch.Draw(texture, buttonRect, colour);
            spriteBatch.DrawString(spriteFont, message, position, Color.White);
        }
    }
}
