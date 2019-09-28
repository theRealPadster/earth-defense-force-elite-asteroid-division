/* ScoreManager.cs
 * Class for an XNA score manager
 * 
 * Revision History:
 *     Isaac Maier, 2015.11.29: Created
 *     Isaac Maier, 2015.12.06: Changed score logic
 *     Isaac Maier, 2015.12.07: Relocated high score logic to here
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
using System.IO;

namespace IMVSFinalProject
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ScoreManager : Microsoft.Xna.Framework.GameComponent
    {
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        CollisionManager cm;

        int currentScore;

        public int CurrentScore
        {
            get { return currentScore; }
            set { currentScore = value; }
        }

        #region fileStuff

        FileStream theFileRead;
        FileStream theFileWrite;
        StreamWriter theScoreWrite;
        StreamReader theScoreRead;
        String[] textHighScoresOld;
        String[] textHighScoresNew;
        int maxHighScores = 5;
        string[] scoreMessages = new string[6];

        public string[] ScoreMessages
        {
            get { return scoreMessages; }
            set { scoreMessages = value; }
        }

        //GetHighScore is only run one time
        Boolean boolHighScoresRun = false;

        public Boolean BoolHighScoresRun
        {
            get { return boolHighScoresRun; }
            set { boolHighScoresRun = value; }
        }

        #endregion

        public ScoreManager(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, CollisionManager cm)
            : base(game)
        {
            //constructor parameters
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
            this.cm = cm;

            //other fields
            this.currentScore = 0;
            this.textHighScoresOld = new String[maxHighScores];
            this.textHighScoresNew = new String[maxHighScores];
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
            if (cm.NewMeteorsShotDown > 0)
            {
                AddPoints(cm.NewMeteorsShotDown * 100);
            }

            if (cm.NewCityHit > 0)
            {
                AddPoints(-cm.NewCityHit * 100);
            }

            base.Update(gameTime);
        }

        public void Draw()
        {
            spriteBatch.DrawString(spriteFont, "Score: " + currentScore, new Vector2(10, 10), Color.White);
        }

        public void AddPoints(int pointsToAdd)
        {
            //add points
            currentScore += pointsToAdd;
        }

        // Get High Scores Method +++++++++++++++++++++++
        // based on code from https://www.youtube.com/watch?v=WfRjGnEEM3M
        internal void GetHighScores()
        {
            Boolean boolWorkingFileIO = true;
            try
            {
                theFileRead = new FileStream("HighScores.txt", FileMode.OpenOrCreate, FileAccess.Read);

                theScoreRead = new StreamReader(theFileRead);

                for (int i = 0; i < maxHighScores; i++)
                {
                    textHighScoresOld[i] = theScoreRead.ReadLine();
                    if (textHighScoresOld[i] == null)
                    {
                        textHighScoresOld[i] = "0";
                    }
                }
                theScoreRead.Close();
                theFileRead.Close();

            }
            catch (Exception)
            {

                boolWorkingFileIO = false;
            }

            if (boolWorkingFileIO)
            {
                int j = 0;

                for (int i = 0; i < maxHighScores; i++)
                {
                    if (currentScore > Convert.ToInt32(textHighScoresOld[i]) && i == j)
                    {
                        textHighScoresNew[i] = currentScore.ToString();
                        i++;

                        if (i < maxHighScores)
                        {
                            textHighScoresNew[i] = textHighScoresOld[j];
                        }
                    }
                    else
                    {
                        textHighScoresNew[i] = textHighScoresOld[j];
                    }

                    j++;
                }
            }

            try
            {
                theFileWrite = new FileStream("HighScores.txt", FileMode.Create, FileAccess.Write);

                theScoreWrite = new StreamWriter(theFileWrite);

                for (int i = 0; i < maxHighScores; i++)
                {
                    theScoreWrite.WriteLine(textHighScoresNew[i]);
                }

                theScoreWrite.Close();
                theFileWrite.Close();

            }
            catch (Exception)
            {

                boolWorkingFileIO = false;
            }

            boolHighScoresRun = true;
        }

        // Read High Scores Method +++++++++++++++++++++++
        internal void ReadHighScores()
        {
            try
            {   //populate old high scores list
                theFileRead = new FileStream("HighScores.txt", FileMode.OpenOrCreate, FileAccess.Read);

                theScoreRead = new StreamReader(theFileRead);

                for (int i = 0; i < maxHighScores; i++)
                {
                    textHighScoresOld[i] = theScoreRead.ReadLine();
                    if (textHighScoresOld[i] == null)
                    {
                        textHighScoresOld[i] = "0";
                    }
                    //set to i+1 since index 0 is the title text
                    scoreMessages[i + 1] = textHighScoresOld[i];
                }
                theScoreRead.Close();
                theFileRead.Close();

            }
            catch (Exception)
            {
                scoreMessages[0] = "High Scores (Broken)";
            }
        }
    }
}
