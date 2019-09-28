/* Game1.cs
 * Main game logic for meteor defense game
 * 
 * Revision History:
 *     Isaac Maier, 2015.11.19: Created
 *     Isaac Maier, 2015.11.19: Added keyboard and mouse state tracking, button clicking
 *     Isaac Maier, 2015.11.24: Added lists for multiple meteors and shots
 *     Isaac Maier, 2015.11.25: Added collision manager, list for multiple explosions,
 *         restrict shots to two on-screen at once
 *     Isaac Maier, 2015.11.26: Disallowed animation of turret when max shots fired,
 *         Added highscore, how to play, and about screens
 *     Vivek Shyam, 2015.11.26: Added parallax scrolling menu background
 *     Vivek Shyam, 2015.11.28: Added buildings to menu background, added menu music
 *     Isaac Maier, 2015.11.29: Added score in corner, crappy dynamic meteor spawning
 *     Isaac Maier, 2015.12.02: Better dynamic meteor spawning
 *     Vivek Shyam, 2015.12.04: Added basic high score system
 *     Isaac Maier, 2015.12.06: Improved GameOver gamestate, implemented game restarts
 *     Vivek Shyam, Isaac Maier: 2015.12.06: Finished high score system,
 *         added difficulties, added regsions, cleaned up some code
 *     Isaac Maier, 2015.12.07: Relocated high score logic to ScoreManager,
 *         played nuclear alarm sound on each new game
 *     Isaac Maier, 2015.12.08: Changed stop menu music to pause while playing, 
 *         added game name, split help and howToPlay into two buttons and screens,
 *         changed Update() switch to an if (combine all textscreens into one)
 *     Isaac Maier, 2015.12.09: Changed ground texture to flat grey
 *     Vivek Shyam, 2015.12.09: Added game over background image
 *     Vivek Shyam, 2015.12.10: Added game over sounds
 *     Isaac Maier, 2015.12.11: Fixed GameOver so doesn't play menu sound or loop explosion
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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Vector2 screenSize;
        SpriteBatch spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        SpriteFont headingFont;
        SpriteFont bodyFont;
        Texture2D whitePixel;

        #region backgrounds

        Texture2D skyBgTexture;
        Texture2D starsTexture;
        ScrollingBackground sbMainBackGround;
        ScrollingBackground sbStarBackGround;
        ScrollingBackground sbStarBackGround2;

        Texture2D buildingImage;
        Texture2D endGameImage;

        #endregion

        #region sounds

        SoundEffect menuIntroSound;
        SoundEffectInstance menuSoundInstance;
        SoundEffect missileLaunch;
        SoundEffectInstance missileLaunchInstance;
        SoundEffect asteroidExplosion;
        SoundEffect nuclearAlarm;
        SoundEffectInstance nuclearAlarmInstance;
        bool nuclearAlarmPlayer = true;
        bool nuclearExplosionPlayer = true;
        SoundEffect nuclearExplosion;
        SoundEffectInstance nuclearExplosionInstance;
        SoundEffect gameOverMusic;
        SoundEffectInstance gameOverMusicInstance;

        #endregion

        int MAX_SHOTS = 2;

        #region buttons

        Button startBtn;
        Button scoresBtn;
        Button howToPlayBtn;
        Button helpBtn;
        Button aboutBtn;
        Button quitBtn;
        Button menuBtn;
        Button easyBtn;
        Button mediumBtn;
        Button hardBtn;

        #endregion

        Texture2D meteorTexture;

        int TIME_COUNTER_MAX;
        int timeCounter;
        bool timeReached;

        Texture2D turretTexture;
        Texture2D turretMountTexture;
        Turret turret;

        Texture2D shotTexture;
        List<Shot> shotList;

        List<Meteor> meteorList;

        Texture2D cityTexture;
        List<City> cityList;

        //TODO - fix this...
        public static Texture2D airExplosion;
        public static Texture2D groundExplosion;
        List<Explosion> explosionList;

        CollisionManager cm;
        ScoreManager sm;

        GameState gameState;
        Difficulty difficulty;
        MouseState mouseState;
        MouseState previousMouseState;
        KeyboardState keyboardState;
        KeyboardState previousKeyboardState;

        TextScreen scoreScreen;
        TextScreen howToPlayScreen;
        TextScreen helpScreen;
        TextScreen aboutScreen;

        enum GameState
        {
            Menu,
            Playing,
            GameOver,
            About,
            Help,
            HowToPlay,
            HighScores,
            GameStart
        }

        enum Difficulty
        {
            Easy,
            Medium,
            Hard,
            NewGame 
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";
            Content = new ResourceContentManager(this.Services, ContentResources.ResourceManager);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {            
            gameState = GameState.Menu;
            IsMouseVisible = true;

            mouseState = Mouse.GetState();
            previousMouseState = mouseState;

            keyboardState = Keyboard.GetState();
            previousKeyboardState = keyboardState;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            screenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            headingFont = Content.Load<SpriteFont>("HeadingFont");
            bodyFont = Content.Load<SpriteFont>("BodyFont");
            whitePixel = Content.Load<Texture2D>("whitePixel");

            #region initializeBackgrounds
            skyBgTexture = Content.Load<Texture2D>("backgroundSky_double_flipped");
            starsTexture = Content.Load<Texture2D>("stars");

            Rectangle mainBackGroundRectangle = new Rectangle(0, 0, skyBgTexture.Width, skyBgTexture.Height);
            Vector2 mainBackGroundPosition = new Vector2(0, 0);
            sbMainBackGround = new ScrollingBackground(this, spriteBatch, skyBgTexture,
                mainBackGroundRectangle, mainBackGroundPosition, new Vector2(0.3F, 0)); //0.2F

            Rectangle starsBackGroundRectangle = new Rectangle(0, 0, starsTexture.Width, starsTexture.Height);
            Vector2 starsBackGroundPosition = new Vector2(0, 0);
            sbStarBackGround = new ScrollingBackground(this, spriteBatch, starsTexture,
                starsBackGroundRectangle, starsBackGroundPosition, new Vector2(0.6F, 0)); //0.5F

            Rectangle starsBackGroundRectangle2 = new Rectangle(0, 0, starsTexture.Width, starsTexture.Height);
            Vector2 starsBackGroundPosition2 = new Vector2(50,-50);
            sbStarBackGround2 = new ScrollingBackground(this, spriteBatch, starsTexture,
                starsBackGroundRectangle2, starsBackGroundPosition2, new Vector2(0.2F, 0)); //0.8F

            buildingImage = Content.Load<Texture2D>("buildings_bigger_2048");
            endGameImage = Content.Load<Texture2D>("nuclear_holocaust");

            #endregion

            #region initializeSounds

            menuIntroSound = Content.Load<SoundEffect>("MenuScreenSoundV1");
            menuSoundInstance = menuIntroSound.CreateInstance();
            menuSoundInstance.Volume = 0.5f;
            missileLaunch = Content.Load<SoundEffect>("MissileLaunch");
            missileLaunchInstance = missileLaunch.CreateInstance();
            missileLaunchInstance.Volume = 0.1f;
            asteroidExplosion = Content.Load<SoundEffect>("AstroidExplosion");
            nuclearAlarm = Content.Load<SoundEffect>("NuclearAlarm");
            nuclearAlarmInstance = nuclearAlarm.CreateInstance();
            nuclearAlarmInstance.Volume = 0.3f;
            nuclearExplosion = Content.Load<SoundEffect>("nuclear_explosion___sound_effect");
            nuclearExplosionInstance = nuclearExplosion.CreateInstance();
            nuclearExplosionInstance.Volume = 1f;
            nuclearExplosionInstance.IsLooped = false;
            gameOverMusic = Content.Load<SoundEffect>("Dramatic_Music");
            gameOverMusicInstance = gameOverMusic.CreateInstance();
            gameOverMusicInstance.Volume = 0.5f;

            #endregion

            #region initializeButtons

            string startBtnMessage = "Start";
            Vector2 startBtnPosition = new Vector2(100, 100);
            startBtn = new Button(this, spriteBatch, headingFont, whitePixel, startBtnMessage, startBtnPosition, Color.MidnightBlue);

            string scoresBtnMessage = "High Scores";
            Vector2 scoresBtnPosition = new Vector2(100, 150);
            scoresBtn = new Button(this, spriteBatch, headingFont, whitePixel, scoresBtnMessage, scoresBtnPosition, Color.SteelBlue);

            string howToPlayBtnMessage = "How to Play";
            Vector2 howToPlayBtnPosition = new Vector2(100, 200);
            howToPlayBtn = new Button(this, spriteBatch, headingFont, whitePixel, howToPlayBtnMessage, howToPlayBtnPosition, Color.SteelBlue);

            string helpBtnMessage = "Help";
            Vector2 helpBtnPosition = new Vector2(100, 250);
            helpBtn = new Button(this, spriteBatch, headingFont, whitePixel, helpBtnMessage, helpBtnPosition, Color.SteelBlue);

            string aboutBtnMessage = "About";
            Vector2 aboutBtnPosition = new Vector2(100, 300);
            aboutBtn = new Button(this, spriteBatch, headingFont, whitePixel, aboutBtnMessage, aboutBtnPosition, Color.SteelBlue);

            string quitBtnMessage = "Quit";
            Vector2 quitBtnPosition = new Vector2(100, 350);
            quitBtn = new Button(this, spriteBatch, headingFont, whitePixel, quitBtnMessage, quitBtnPosition, Color.SteelBlue);

            string menuBtnMessage = "Menu";
            Vector2 menuBtnPosition = new Vector2(100, screenSize.Y - 100);
            menuBtn = new Button(this, spriteBatch, headingFont, whitePixel, menuBtnMessage, menuBtnPosition, Color.SteelBlue);

            string easyBtnMessage = "Easy";
            Vector2 easyBtnPosition = new Vector2(100, 100);
            easyBtn = new Button(this, spriteBatch, headingFont, whitePixel, easyBtnMessage, easyBtnPosition, Color.SteelBlue);

            string mediumBtnMessage = "Medium";
            Vector2 mediumBtnPosition = new Vector2(100, 150);
            mediumBtn = new Button(this, spriteBatch, headingFont, whitePixel, mediumBtnMessage, mediumBtnPosition, Color.SteelBlue);

            string hardBtnMessage = "Hard";
            Vector2 hardBtnPosition = new Vector2(100, 200);
            hardBtn = new Button(this, spriteBatch, headingFont, whitePixel, hardBtnMessage, hardBtnPosition, Color.SteelBlue);

            #endregion

            //delay between meteors, change this for difficulty levels? (not ALL_CAPS then?...)
            TIME_COUNTER_MAX = 100;

            meteorTexture = Content.Load<Texture2D>("trail_4");
            meteorList = new List<Meteor>();

            turretTexture = Content.Load<Texture2D>("turret_sheet");
            turretMountTexture = Content.Load<Texture2D>("turret_mount_new");
            turret = new Turret(this, spriteBatch, turretTexture, turretMountTexture, screenSize);

            shotTexture = Content.Load<Texture2D>("rocket_spritesheet_vertical");
            shotList = new List<Shot>();

            cityTexture = Content.Load<Texture2D>("city_small_sprite");

            cityList = new List<City>();

            airExplosion = Content.Load<Texture2D>("air_explosion");
            groundExplosion = Content.Load<Texture2D>("ground_explosion");

            explosionList = new List<Explosion>();

            cm = new CollisionManager(this, shotList, meteorList, explosionList, cityList, screenSize, asteroidExplosion);
            sm = new ScoreManager(this, spriteBatch, headingFont, cm);

            sm.ScoreMessages[0] = "High Scores";
            for (int i = 1; i < sm.ScoreMessages.Length; i++)
            {
                sm.ScoreMessages[i] = "score " + i.ToString();
            }

            scoreScreen = new TextScreen(this, spriteBatch, headingFont, bodyFont, sm.ScoreMessages);

            string[] howToPlayMessages = { "How to Play", "Aim with the mouse", "Click to shoot", "Protect the cities" };
            howToPlayScreen = new TextScreen(this, spriteBatch, headingFont, bodyFont, howToPlayMessages);

            string[] helpMessages = { "Help", 
                                    "The concept of the arcade game EDF:EAD, is for the player to stop the asteroids that rain from the sky from destroying the cities below.", 
                                    "The player will be able to stop the asteroids from hitting the cities by firing a missile directly at the asteroid to be destroyed", 
                                    "before hitting the cities.",
                                    "The game is played by moving the mouse and clicking on the anticipated location of the asteroids and firing a missile", 
                                    "directly at the asteroids. ",
                                    "",
                                    "100 points are gained for each asteroid shot down.",
                                    "100 points are lost for each city hit.",
                                    "300 points are lost for each city destroyed." };
            helpScreen = new TextScreen(this, spriteBatch, headingFont, bodyFont, helpMessages);

            string[] aboutMessages = { "About", "Created by Isaac Maier and Vivek Shyam as a final project for Object Oriented Game Programming in 2015", 
                                        "Earth Defence Force: Elite Asteroid Division is based on the popular arcade game from the 1980s called Missile Command by Atari Inc.",
                                        "Assets attributed in help documentation." };
            aboutScreen = new TextScreen(this, spriteBatch, headingFont, bodyFont, aboutMessages);

            //contains logic common to resetting game state
            restartGame();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            UpdateMenuBackground(gameTime);

            if (gameState == GameState.Menu)
            {
                if (previousKeyboardState.IsKeyDown(Keys.Escape)
                        && keyboardState.IsKeyUp(Keys.Escape))
                {
                    this.Exit();
                }

                nuclearExplosionInstance.Stop();
                gameOverMusicInstance.Stop();

                menuSoundInstance.Play();

                startBtn.Update(gameTime, mouseState, previousMouseState);
                scoresBtn.Update(gameTime, mouseState, previousMouseState);
                howToPlayBtn.Update(gameTime, mouseState, previousMouseState);
                helpBtn.Update(gameTime, mouseState, previousMouseState);
                aboutBtn.Update(gameTime, mouseState, previousMouseState);
                quitBtn.Update(gameTime, mouseState, previousMouseState);

                if (startBtn.IsClicked)
                {
                    gameState = GameState.GameStart;
                }
                else if (scoresBtn.IsClicked)
                {
                    gameState = GameState.HighScores;
                }
                else if (howToPlayBtn.IsClicked)
                {
                    gameState = GameState.HowToPlay;
                }
                else if (helpBtn.IsClicked)
                {
                    gameState = GameState.Help;
                }
                else if (aboutBtn.IsClicked)
                {
                    gameState = GameState.About;
                }
                else if (quitBtn.IsClicked)
                {
                    this.Exit();
                }
            }
            else if (gameState == GameState.GameStart)
            {
                //if new game
                if (difficulty == Difficulty.NewGame)
                {
                    sm.BoolHighScoresRun = false;
                    menuBtn.Update(gameTime, mouseState, previousMouseState);
                    easyBtn.Update(gameTime, mouseState, previousMouseState);
                    mediumBtn.Update(gameTime, mouseState, previousMouseState);
                    hardBtn.Update(gameTime, mouseState, previousMouseState);

                    if (previousKeyboardState.IsKeyDown(Keys.Escape)
                        && keyboardState.IsKeyUp(Keys.Escape))
                    {
                        gameState = GameState.Menu;
                    }
                    else if (menuBtn.IsClicked)
                    {
                        gameState = GameState.Menu;
                    }
                    else if (easyBtn.IsClicked)
                    {
                        difficulty = Difficulty.Easy;
                        MAX_SHOTS = 4;
                        TIME_COUNTER_MAX = 200;
                        gameState = GameState.Playing;
                    }
                    else if (mediumBtn.IsClicked)
                    {
                        difficulty = Difficulty.Medium;
                        MAX_SHOTS = 2;
                        TIME_COUNTER_MAX = 100;
                        gameState = GameState.Playing;
                    }
                    else if (hardBtn.IsClicked)
                    {
                        difficulty = Difficulty.Hard;
                        MAX_SHOTS = 1;
                        TIME_COUNTER_MAX = 75;
                        gameState = GameState.Playing;
                    }

                    menuSoundInstance.Play();
                }
                else
                {
                    gameState = GameState.Playing;
                }
            }
            else if (gameState == GameState.Playing)
            {
                if (previousKeyboardState.IsKeyDown(Keys.Escape)
                        && keyboardState.IsKeyUp(Keys.Escape))
                {
                    gameState = GameState.Menu;
                }

                timeCounter++;

                if (timeCounter > TIME_COUNTER_MAX)
                {
                    timeReached = true;
                    timeCounter = 0;
                }

                UpdateMenuBackground(gameTime);

                menuSoundInstance.Pause();

                if (nuclearAlarmPlayer)
                {
                    nuclearAlarmInstance.Play();
                    nuclearAlarmPlayer = false;
                }

                if (timeReached)
                {
                    Meteor newMeteor = new Meteor(this, spriteBatch, meteorTexture, screenSize);
                    meteorList.Add(newMeteor);
                }

                for (int i = 0; i < meteorList.Count; i++)
                {
                    meteorList[i].Update(gameTime);
                }

                turret.Update(gameTime, mouseState, previousMouseState);

                if (turret.IsClicked)
                {
                    //only make new shot if under limit
                    if (shotList.Count < MAX_SHOTS)
                    {
                        Shot shot = new Shot(this, spriteBatch, shotTexture, new Vector2(mouseState.X, mouseState.Y), turret, screenSize, missileLaunchInstance);
                        shotList.Add(shot);
                        turret.DisplayAnim = true;
                    }
                    else
                    {
                        turret.DisplayAnim = false;
                    }
                }

                for (int i = 0; i < shotList.Count; i++)
                {
                    shotList[i].Update(gameTime);
                }

                cm.Update(gameTime);

                for (int i = 0; i < explosionList.Count; i++)
                {
                    //just animation
                    explosionList[i].Update(gameTime);
                    if (explosionList[i].AnimationOver)
                    {
                        explosionList[i] = null;
                        explosionList.RemoveAt(i);
                        i--;
                    }
                }

                sm.Update(gameTime);

                timeReached = false;

                if (cityList.Count() == 0)
                {
                    gameState = GameState.GameOver;
                }
            }
            else if (gameState == GameState.HighScores
                    || gameState == GameState.HowToPlay
                    || gameState == GameState.Help
                    || gameState == GameState.About)
            {
                menuBtn.Update(gameTime, mouseState, previousMouseState);
                if (previousKeyboardState.IsKeyDown(Keys.Escape)
                    && keyboardState.IsKeyUp(Keys.Escape))
                {
                    gameState = GameState.Menu;
                }
                else if (menuBtn.IsClicked)
                {
                    gameState = GameState.Menu;
                }

                menuSoundInstance.Play();
            }
            else if (gameState == GameState.GameOver)
            {
                menuBtn.Update(gameTime, mouseState, previousMouseState);

                if (!sm.BoolHighScoresRun)
                {
                    sm.GetHighScores();
                }                

                if (nuclearExplosionPlayer)
                {
                    nuclearExplosionInstance.Play();
                }

                nuclearExplosionPlayer = false;

                gameOverMusicInstance.Play();
                
                if (previousKeyboardState.IsKeyDown(Keys.Escape)
                    && keyboardState.IsKeyUp(Keys.Escape))
                {
                    gameState = GameState.Menu;
                    restartGame();
                }
                else if (menuBtn.IsClicked)
                {
                    gameState = GameState.Menu;
                    restartGame();
                }
            }

            previousKeyboardState = keyboardState;
            previousMouseState = mouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MidnightBlue);

            spriteBatch.Begin();

            DrawMenuBackground();

            switch (gameState)
            {
                case GameState.Menu:
                {
                    DrawGame();
                    DrawTitleText();

                    startBtn.Draw();
                    scoresBtn.Draw();
                    howToPlayBtn.Draw();
                    helpBtn.Draw();
                    aboutBtn.Draw();
                    quitBtn.Draw();

                    break;
                }
                case GameState.GameStart:
                {
                    DrawGame();

                    if (difficulty == Difficulty.NewGame)
                    {
                        DrawTitleText();
                        easyBtn.Draw();
                        mediumBtn.Draw();
                        hardBtn.Draw();
                        menuBtn.Draw();
                    }
                    
                    break;
                }
                case GameState.Playing:
                {
                    DrawGame();

                    break;
                }
                case GameState.HighScores:
                {
                    scoreScreen.Draw();
                    DrawTitleText();
                    menuBtn.Draw();
                    break;
                }
                case GameState.HowToPlay:
                {
                    howToPlayScreen.Draw();
                    DrawTitleText();
                    menuBtn.Draw();
                    break;
                }
                case GameState.Help:
                {
                    helpScreen.Draw();
                    DrawTitleText();
                    menuBtn.Draw();
                    break;
                }
                case GameState.About:
                {
                    aboutScreen.Draw();
                    DrawTitleText();
                    menuBtn.Draw();
                    break;
                }
                case GameState.GameOver:
                {
                    spriteBatch.Draw(endGameImage, new Rectangle((int)(screenSize.X - endGameImage.Width)/2, 0, endGameImage.Width, endGameImage.Height), Color.White);
                    scoreScreen.Draw();
                    DrawTitleText();
                    menuBtn.Draw();
                    break;
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGame()
        {
            spriteBatch.Draw(whitePixel, new Rectangle(0, (int)screenSize.Y - 20, (int)screenSize.X, 20), Color.DarkSlateGray);

            for (int i = 0; i < cityList.Count; i++)
            {
                cityList[i].Draw();
            }

            for (int i = 0; i < meteorList.Count; i++)
            {
                meteorList[i].Draw();
            }

            for (int i = 0; i < shotList.Count; i++)
            {
                shotList[i].Draw();
            }

            turret.Draw();

            for (int i = 0; i < explosionList.Count; i++)
            {
                explosionList[i].Draw();
            }

            sm.Draw();
        }

        private void DrawMenuBackground()
        {
            sbMainBackGround.Draw();
            sbStarBackGround.Draw();
            sbStarBackGround2.Draw();
            spriteBatch.Draw(buildingImage, new Vector2(0, screenSize.Y - buildingImage.Height), new Rectangle(0, 0, buildingImage.Width, buildingImage.Height), Color.Black, 0F, new Vector2(), 1F, SpriteEffects.None, 0F);
        }

        private void UpdateMenuBackground(GameTime gameTime)
        {
            sbMainBackGround.Update(gameTime);
            sbStarBackGround.Update(gameTime);
            sbStarBackGround2.Update(gameTime);
        }

        private void DrawTitleText()
        {
            spriteBatch.DrawString(headingFont, "Earth Defence Force: Elite Asteroid Division", new Vector2(screenSize.X / 2 - 250, 10), Color.LightSkyBlue);
        }

        private void restartGame()
        {
            difficulty = Difficulty.NewGame;

            nuclearAlarmPlayer = true;
            nuclearExplosionPlayer = true;

            //read high scores before resetting game
            sm.ReadHighScores();

            sm.CurrentScore = 0;

            for (int i = 0; i < cityList.Count(); i++)
            {
                cityList[i] = null;
                cityList.RemoveAt(i);
                i--;
            }

            for (int i = 0; i < meteorList.Count(); i++)
            {
                meteorList[i] = null;
                meteorList.RemoveAt(i);
                i--;
            }

            for (int i = 0; i < shotList.Count(); i++)
            {
                shotList[i] = null;
                shotList.RemoveAt(i);
                i--;
            }

            for (int i = 0; i < explosionList.Count(); i++)
            {
                explosionList[i] = null;
                explosionList.RemoveAt(i);
                i--;
            }

            timeCounter = 0;
            timeReached = false;

            turret = null;
            turret = new Turret(this, spriteBatch, turretTexture, turretMountTexture, screenSize);

            //spaced out in 5ths
            City city1 = new City(this, spriteBatch, cityTexture, (int)(0 * screenSize.X / 10) + cityTexture.Width / 3 / 4, screenSize);
            City city2 = new City(this, spriteBatch, cityTexture, (int)(2 * screenSize.X / 10) + cityTexture.Width / 3 / 4, screenSize);
            City city3 = new City(this, spriteBatch, cityTexture, (int)(6 * screenSize.X / 10) + cityTexture.Width / 3 / 4, screenSize);
            City city4 = new City(this, spriteBatch, cityTexture, (int)(8 * screenSize.X / 10) + cityTexture.Width / 3 / 4, screenSize);

            cityList.Add(city1);
            cityList.Add(city2);
            cityList.Add(city3);
            cityList.Add(city4);
        }
    }
}
