/* TextScreen.cs
 * Class for an XNA text display screen
 * 
 * Revision History:
 *     Isaac Maier, 2015.11.26: Created
 *     Isaac Maier, 2015.12.09: Added body/header fonts, moved body text down
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
    public class TextScreen : Microsoft.Xna.Framework.GameComponent
    {    
        SpriteBatch spriteBatch;
        SpriteFont headingFont;
        SpriteFont bodyFont;
        string[] messages;
        Vector2 titlePos;

        public TextScreen(Game game, SpriteBatch spriteBatch, SpriteFont headingFont, SpriteFont bodyFont, string[] messages)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.messages = messages;
            this.headingFont = headingFont;
            this.bodyFont = bodyFont;

            this.titlePos = new Vector2(600,50); //TODO - this
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
            base.Update(gameTime);
        }

        public void Draw()
        {
            spriteBatch.DrawString(headingFont, messages[0], titlePos, Color.White);

            for(int i = 1; i < messages.Length; i++)
            {
                spriteBatch.DrawString(bodyFont, messages[i], new Vector2(100, 50 + 50*i), Color.White);
            }
        }
    }
}
