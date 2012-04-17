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
    /// This Parallax sprite can be moved "manually", meaning it can be coupled to
    /// other game actions' velocities like the player's movements and actions.
    /// 
    /// Author: Henrique Gorni (gorni.henrique@gmail.com)
    /// URL: http://www.demiurgo.com.br
    /// </summary>
    public class ParallaxMovableSprite : ParallaxBaseSprite
    {
        /// <summary>
        /// Initializes the movable sprite
        /// </summary>
        /// <param name="texture">texture to use with the sprite</param>
        /// <param name="basePosition">initial/base position of the sprite</param>
        /// <param name="bounds">screen/client bounds</param>
        /// <param name="hBuffer">X-axis buffer; this parameter defines how many sprites need
        /// to be aligned horizontally to create a transition smooth enough to avoid screen gaps;
        /// 2-3 is generally a good number</param>
        /// <param name="vBuffer">Y-axis buffer; this parameter defines how many sprites need
        /// to be aligned vertically to create a transition smooth enough to avoid screen gaps;
        /// note that the horizontal sprite already counts as 1 vertical sprite; 2-3 is generally
        /// a good number</param>
        /// <param name="scale">scale of the sprite to be drawn (1.0 = 100%)</param>
        /// <param name="velocityRatio">defines the ratio of the velocity used to move the sprite;
        /// 1 = 100%; e.g. player is moving at (x,y) = (5,2) and velocity ratio is 0.5, then the
        /// sprite velocity is (x,y) = (2.5,1)</param>
        /// <param name="flipVelocity">flips the velocity every time the velocity is updated; 
        /// e.g. player is moving at (x,y) = (5,2) and flip velocity is set to true, then the
        /// sprite velocity is (x,y) = (-5,-2)</param>
        /// <param name="layerDepth">layer depth at which the sprite will be drawn</param>
        public ParallaxMovableSprite(Texture2D texture, Vector2 basePosition, Rectangle bounds, int hBuffer = 2,
            int vBuffer = 1, float scale = 1f, float velocityRatio = 1f, bool flipVelocity = true, float layerDepth = 0)
            : base(texture, basePosition, bounds, hBuffer, vBuffer, scale, layerDepth)
        {
            this.velocityRatio = velocityRatio;
            this.flipVelocity = flipVelocity;
        }

        /// <summary>
        /// Updates the sprite and buffers at each update cycle on the X-axis and Y-axis
        /// </summary>
        /// <param name="gameTime">the GameTime instance, usually retrieved from the Game instance</param>
        public override void Update(GameTime gameTime)
        {
            // Ignore update calls if disabled
            if (!enabled) return;

            // Update positions e move sprites around to keep them 
            // on the drawable section of the screen
            UpdateXAxis();
            UpdateYAxis();

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the sprite and buffers plus the velocity at each update cycle on the X-axis and Y-axis
        /// </summary>
        /// <param name="gameTime">the GameTime instance, usually retrieved from the Game instance</param>
        /// <param name="velocity">velocity used to move the sprite; e.g. player's velocity</param>
        public void Update(GameTime gameTime, Vector2 velocity)
        {
            SetVelocity(velocity);
            this.Update(gameTime);
        }

        /// <summary>
        /// Updates X-axis sprites positions and moves X-axis buffers around
        /// </summary>
        protected void UpdateXAxis()
        {
            // Return if X-axis is locked
            if (lockXAxis) return;

            // Recalculate horizontal (X) positions e move sprites around the X-axis 
            // to avoid gaps in the drawable section of the screen
            for (int j = 0; j < positions.Length; ++j)
            {
                // Update velocity
                positions[j].X += velocity.X;

                // Adjust offscreen sprites
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

        /// <summary>
        /// Updates Y-axis sprites positions and moves Y-axis buffers around
        /// </summary>
        protected void UpdateYAxis()
        {
            // Return if Y-axis is locked
            if (lockYAxis) return;

            // Recalculate vertical (Y) positions e move sprites around the Y-axis 
            // to avoid gaps in the drawable screen
            for (int j = 0; j < positions.Length; ++j)
            {
                // Update velocity
                positions[j].Y += velocity.Y;

                // Adjust offscreen sprites
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

        /// <summary>
        /// Sets the velocity and applies the velocity ratio
        /// </summary>
        /// <param name="velocity">velocity to be set for the sprite's movement</param>
        private void SetVelocity(Vector2 velocity)
        {
            this.velocity = flipVelocity ? -velocity : velocity;
            this.velocity *= velocityRatio;
        }

        // Velocity
        protected float velocityRatio;
        protected Vector2 velocity = Vector2.Zero;
        protected bool flipVelocity;
        protected bool lockXAxis = false;
        protected bool lockYAxis = false;

        // Attributes
        public float VelocityRatio
        {
            get { return velocityRatio; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { SetVelocity(value); }
        }

        public bool FlipVelocity
        {
            get { return flipVelocity; }
            set { flipVelocity = value; }
        }

        /// <summary>
        /// If locked, X-axis can't be moved and any updates to X-axis are ignored
        /// </summary>
        public bool LockXAxis
        {
            get { return lockXAxis; }
            set { lockXAxis = value; }
        }

        /// <summary>
        /// If locked, Y-axis can't be moved and any updates to Y-axis are ignored
        /// </summary>
        public bool LockYAxis
        {
            get { return lockYAxis; }
            set { lockYAxis = value; }
        }
    }


}