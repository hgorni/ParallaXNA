using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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
    /// This is a Parallax sprite has automatic movement, idependent of player's movements/inputs.
    /// Ideal for scenario dynamic scenario items like clouds, star fields in space shooters, etc.
    /// 
    /// Author: Henrique Gorni (gorni.henrique@gmail.com)
    /// URL: http://www.demiurgo.com.br
    /// </summary>
    public class ParallaxAutomaticSprite : ParallaxBaseSprite
    {
        /// <summary>
        /// Initializes the automatic sprite.
        /// </summary>
        /// <param name="texture">texture to use with the sprite</param>
        /// <param name="basePosition">initial/base position of the sprite</param>
        /// <param name="bounds">screen/client bounds</param>
        /// <param name="velocity">constant velocity at which the sprite will move</param>
        /// <param name="hBuffer">X-axis buffer; this parameter defines how many sprites need
        /// to be aligned horizontally to create a transition smooth enough to avoid screen gaps;
        /// 2-3 is generally a good number</param>
        /// <param name="vBuffer">Y-axis buffer; this parameter defines how many sprites need
        /// to be aligned vertically to create a transition smooth enough to avoid screen gaps;
        /// note that the horizontal sprite already counts as 1 vertical sprite; 2-3 is generally
        /// a good number</param>
        /// <param name="scale">scale of the sprite to be drawn (1.0 = 100%)</param>
        /// <param name="layerDepth">layer depth at which the sprite will be drawn</param>
        public ParallaxAutomaticSprite(Texture2D texture, Vector2 basePosition, Rectangle bounds,
                    Vector2 velocity, int hBuffer = 2, int vBuffer = 1, float scale = 1f, float layerDepth = 0)
            : base(texture, basePosition, bounds, hBuffer, vBuffer, scale, layerDepth)
        {
            Velocity = velocity;
        }

        public override void Update(GameTime gameTime)
        {
            if (!enabled) return;

            // Update positions
            for (int i = 0; i < positions.Length; ++i)
                positions[i] += velocity;

            // Update positions e move sprites around to keep them 
            // on the drawable section of the screen
            UpdateXAxis();
            UpdateYAxis();

            base.Update(gameTime);
        }

        protected void UpdateXAxis()
        {
            // Recalculate horizontal (X) positions e move sprites around the X-axis 
            // to avoid gaps in the drawable section of the screen
            for (int j = 0; j < positions.Length; ++j)
            {
                if (positions[j].X + wTextureScaled <= 0)
                {
                    positions[j].X += hLenScaled;
                }
                else if (positions[j].X >= bounds.Width)
                {
                    positions[j].X -= hLenScaled;
                }
            }
        }

        protected void UpdateYAxis()
        {
            // Recalculate vertical (Y) positions e move sprites around the Y-axis 
            // to avoid gaps in the drawable screen
            for (int j = 0; j < positions.Length; ++j)
            {
                if (positions[j].Y + hTextureScaled <= 0)
                {
                    positions[j].Y += vLenScaled;
                }
                else if (positions[j].Y >= bounds.Height)
                {
                    positions[j].Y -= vLenScaled;
                }
            }
        }

        // Velocity
        protected Vector2 velocity = Vector2.Zero;

        // Attributes
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
    }
}