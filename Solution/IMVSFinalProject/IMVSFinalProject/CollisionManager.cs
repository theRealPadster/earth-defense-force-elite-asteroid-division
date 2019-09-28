/* CollisionManager.cs
 * Class for an XNA collision manager
 * 
 * Revision History:
 *     Isaac Maier, 2015.11.24: Created
 *     Isaac Maier, 2015.11.25: Added remove offscreen shots and meteors,
 *         also really, really crappy shot-meteor collision detection
 *     Isaac Maier, 2015.11.30: Added really crappy city-meteor collision detection
 *     Isaac Maier, 2015.12.02: Added states to city destruction, and deletion
 *     Isaac Maier, 2015.12.03: Fixed crashes on certain collisions, now using GetRect()
 *     Isaac Maier, 2015.12.06: Changed SoundEffectInstance to SoundEffect,
 *         changed score penalty for hitting cities vs destroying cities
 *     Isaac Maier, 2015.12.09: Made city destroy explosions use ground explosion
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
    public class CollisionManager : Microsoft.Xna.Framework.GameComponent
    {
        Game1 game;
        List<Shot> shotList;
        List<Meteor> meteorList;
        List<Explosion> explosionList;
        List<City> cityList;
        Vector2 screenSize;

        SoundEffect explosionSoundEffect;
        int newMeteorsShotDown;
        int newCityHit;

        public int NewMeteorsShotDown
        {
            get { return newMeteorsShotDown; }
        }

        public int NewCityHit
        {
            get { return newCityHit; }
        }

        public CollisionManager(Game game, List<Shot> shotList, List<Meteor> meteorList, List<Explosion> explosionList, List<City> cityList, Vector2 screenSize, SoundEffect explosionSoundEffect)
            : base(game)
        {
            this.game = (Game1) game;
            this.shotList = shotList;
            this.meteorList = meteorList;
            this.explosionList = explosionList;
            this.cityList = cityList;
            this.screenSize = screenSize;

            this.explosionSoundEffect = explosionSoundEffect;

            this.newMeteorsShotDown = 0;
            this.newCityHit = 0;
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
            newMeteorsShotDown = 0;
            newCityHit = 0;
            //remove offscreen meteors
            for (int i = 0; i < meteorList.Count; i++)
            {
                if (meteorList[i].Position.Y > screenSize.Y - meteorList[i].Texture.Height)
                {
                    //trigger explosion here
                    //TODO - tweak position given so it looks natural instead of jumping
                    Explosion explosion = new Explosion(game, game.SpriteBatch, meteorList[i].Position, meteorList[i].Texture.Height, screenSize, explosionSoundEffect);
                    explosionList.Add(explosion);
                    meteorList[i] = null;
                    meteorList.RemoveAt(i);
                    i--; //since the count changes in the loop when removed
                }
            }

            for (int j = 0; j < shotList.Count; j++)
            {
                if (shotList[j].Position.Y < 0
                    || shotList[j].Position.Y > screenSize.Y
                    || shotList[j].Position.X < 0
                    || shotList[j].Position.X > screenSize.X)
                {
                    shotList[j] = null;
                    shotList.RemoveAt(j);
                    j--; //since the count changes in the loop when removed
                }
            }

            for (int i = 0; i < meteorList.Count; i++)
            {
                for (int s = 0; s < shotList.Count; s++)
                {
                    if (shotList[s].GetRect().Intersects(meteorList[i].GetRect()))
                    {
                        Explosion explosion = new Explosion(game, game.SpriteBatch, meteorList[i].Position, meteorList[i].Texture.Height, screenSize, explosionSoundEffect);
                        explosionList.Add(explosion);

                        meteorList[i] = null;
                        meteorList.RemoveAt(i);
                        shotList[s] = null;
                        shotList.RemoveAt(s);

                        i--;
                        s--;

                        newMeteorsShotDown++;

                        break;
                    }
                }
            }

            for (int i = 0; i < cityList.Count; i++)
            {
                for (int s = 0; s < meteorList.Count; s++)
                {

                    Rectangle meteorRect = meteorList[s].GetRect();

                    if (meteorRect.Intersects(cityList[i].GetRect()))
                    {
                        cityList[i].GotHit();
                        
                        if (cityList[i].IsDestroyed())
                        {
                            //TODO - remove magic numbers somehow...
                            //Vector2 explosionPosition = new Vector2(cityList[i].Position.X + 40, (screenSize.Y - Game1.groundExplosion.Height / 3));
                            Vector2 explosionPosition = new Vector2(cityList[i].Position.X + 80, screenSize.Y - Game1.groundExplosion.Height / 3);

                            Explosion explosion = new Explosion(game, game.SpriteBatch, explosionPosition, cityList[i].Texture.Height, screenSize, explosionSoundEffect);
                            explosionList.Add(explosion);

                            cityList[i] = null;
                            cityList.RemoveAt(i);
                            i--;
                            meteorList[s] = null;
                            meteorList.RemoveAt(s);
                            s--;
                            //city destruction costs as much as 3 hits
                            newCityHit += 3;
                            break;
                        }
                        else
                        {
                            //TODO - remove magic numbers somehow...
                            //Vector2 explosionPosition = new Vector2(cityList[i].Position.X + 40, (screenSize.Y - Game1.groundExplosion.Height / 3));
                            Vector2 explosionPosition = new Vector2(cityList[i].Position.X + 80, (screenSize.Y - 130));

                            Explosion explosion = new Explosion(game, game.SpriteBatch, explosionPosition, cityList[i].Texture.Height, screenSize, explosionSoundEffect);
                            explosionList.Add(explosion);
                        }
                        
                        meteorList[s] = null;
                        meteorList.RemoveAt(s);

                        s--;

                        newCityHit++;
                    }
                }
            }

            base.Update(gameTime);
        }
    }
}
