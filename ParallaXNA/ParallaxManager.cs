using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

/**
 * This work is licensed under the Creative Commons Attribution-ShareAlike 3.0 Unported License. 
 * To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/3.0/ or send a letter 
 * to Creative Commons, 444 Castro Street, Suite 900, Mountain View, California, 94041, USA.
 * 
 * Este trabalho está licençiado sob a Licença Attribution-ShareAlike 3.0 Unported da Creative Commons. 
 * Para ver uma cópia desta licença, visite http://creativecommons.org/licenses/by-sa/3.0/ ou envie uma 
 * carta para Creative Commons, 444 Castro Street, Suite 900, Mountain View, California, 94041, USA.
 * 
 * Author: Henrique Gorni (gorni.henrique@gmail.com)
 * URL: http://www.demiurgo.com.br
 */
namespace Demiurgo.Component2D.Parallax
{
    /// <summary>
    /// This is the manager class used to draw and updated the Parallax sprites.
    /// At each update cycle the velocity (vector containing direction and speed)
    /// can be updated so that any sprite that has updatable movements are affected.
    /// 
    /// Author: Henrique Gorni (gorni.henrique@gmail.com)
    /// URL: http://www.demiurgo.com.br
    /// </summary>
    public class ParallaxManager : DrawableGameComponent
    {
        public ParallaxManager(Game game)
            : base(game)
        {
            this.game = game;
        }

        /// <summary>
        /// Initializes the manager with a list of Parallax sprites
        /// </summary>
        /// <param name="game">the game instance</param>
        /// <param name="parallaxSprites">list of Parallax sprite instances</param>
        public ParallaxManager(Game game, List<ParallaxBaseSprite> parallaxSprites)
            : base(game)
        {
            this.game = game;
            this.parallaxSprites = parallaxSprites;
        }

        public override void Initialize()
        {


            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Load sprite batch used to draw the sprites
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            base.LoadContent();
        }

        /// <summary>
        /// Updates the sprites at each update cycle. Parallax sprite types with updatable
        /// movements are also updated with velocity.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Update the sprites
            foreach (ParallaxBaseSprite pSprite in parallaxSprites)
            {
                // Sprite is autonomous ?
                if (pSprite.Autonomous)
                {
                    // Update without velocity
                    pSprite.Update(gameTime, game.Window.ClientBounds);
                }
                else 
                {
                    // Update with velocity
                    pSprite.Update(gameTime, game.Window.ClientBounds, this.velocity);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the sprites using the list of positions retrieved from each 
        /// Parallax sprite instance.
        /// </summary>
        /// <param name="gameTime">the game instance</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(spriteSortMode, BlendState.AlphaBlend);

            foreach (ParallaxBaseSprite pSprite in parallaxSprites)
            {
                foreach (Vector2 position in pSprite.GetPositions())
                {
                    spriteBatch.Draw(pSprite.Texture, position, pSprite.Texture.Bounds, Color.White, 0,
                        Vector2.Zero, pSprite.Scale, SpriteEffects.None, pSprite.LayerDepth);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // Fields
        SpriteBatch spriteBatch;
        SpriteSortMode spriteSortMode = SpriteSortMode.BackToFront;
        Game game;

        // Velocity only affects ParallaxMovableSprite instances
        Vector2 velocity = Vector2.Zero;

        // Parallax sprites
        List<ParallaxBaseSprite> parallaxSprites = new List<ParallaxBaseSprite>();

        // Attributes
        /// <summary>
        /// Updates/sets the velocity. Usually updated after each game cycle to
        /// reflect player's movements, camera movements, etc.
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public List<ParallaxBaseSprite> ParallaxSprites
        {
            get { return parallaxSprites; }
            set { parallaxSprites = value; }
        }

        /// <summary>
        /// Sets the SpriteSortMode. This affects how the sprites draw order
        /// is sorted according to their layer depth.
        /// </summary>
        public SpriteSortMode @SpriteSortMode
        {
            get { return spriteSortMode; }
            set { spriteSortMode = value; }
        }
    }
}
