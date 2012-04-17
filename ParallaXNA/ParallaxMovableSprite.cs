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
    /// other game actions' velocities like the player's movements and/or camera.
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
        /// <param name="distanceFromCamera">the distance of the sprite from the camera into the horizon</param>
        /// <param name="screenBounds">the client screen bounds</param>
        /// <param name="viewingAngle">the viewing angle of the camera</param>
        /// <param name="hBuffer">X-axis buffer; this parameter defines how many sprites need
        /// to be aligned horizontally to create a transition smooth enough to avoid screen gaps;
        /// 2-3 is generally a good number</param>
        /// <param name="vBuffer">Y-axis buffer; this parameter defines how many sprites need
        /// to be aligned vertically to create a transition smooth enough to avoid screen gaps;
        /// note that the horizontal sprite already counts as 1 vertical sprite; 2-3 is generally
        /// a good number</param>
        /// <param name="scale">scale of the sprite to be drawn (1.0 = 100%)</param>
        /// <param name="flipDirection">flips the direction every time the velocity is updated; 
        /// e.g. player is moving at (x,y) = (5,2) and flip direction is set to true, then the
        /// sprite velocity is (x,y) = (-5,-2)</param>
        public ParallaxMovableSprite(Texture2D texture, Vector2 basePosition, float distanceFromCamera,
            Rectangle screenBounds, float viewingAngle = MathHelper.PiOver4, int hBuffer = 2, int vBuffer = 1,
            float scale = 1f, bool flipDirection = true)
            : base(texture, basePosition, distanceFromCamera, screenBounds, viewingAngle, hBuffer, vBuffer, scale)
        {
            this.flipDirection = flipDirection;
        }

        /// <summary>
        /// Updates the sprite and buffers at each update cycle on the X-axis and Y-axis
        /// </summary>
        /// <param name="gameTime">the GameTime instance, usually retrieved from the Game instance</param>
        /// <param name="screenBounds">the client screen bounds</param>
        public override void Update(GameTime gameTime, Rectangle screenBounds)
        {
            // Ignore update calls if disabled
            if (!enabled)
                return;
            else
                base.Update(gameTime, screenBounds);

            // Update positions and move sprites around to keep them 
            // on the drawable section of the screen
            UpdateXAxis(screenBounds);
            UpdateYAxis(screenBounds);
        }

        /// <summary>
        /// Updates the sprite and buffers plus the velocity at each update cycle on the X-axis and Y-axis
        /// </summary>
        /// <param name="gameTime">the GameTime instance, usually retrieved from the Game instance</param>
        /// <param name="screenBounds">the client screen bounds</param>
        /// <param name="velocity">velocity used to move the sprite; e.g. player's velocity</param>
        public void Update(GameTime gameTime, Rectangle screenBounds, Vector2 velocity)
        {
            SetVelocity(velocity);
            this.Update(gameTime, screenBounds);
        }

        /// <summary>
        /// Updates X-axis sprites positions and moves X-axis buffers around
        /// </summary>
        /// <param name="screenBounds">the client screen bounds</param>
        protected void UpdateXAxis(Rectangle screenBounds)
        {
            // Return if X-axis is locked
            if (lockXAxis) return;

            // Recalculate horizontal (X) positions and move sprites around the X-axis 
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
                else if (positions[j].X >= screenBounds.Width)
                {
                    positions[j].X -= hLenScaled;
                }
            }
        }

        /// <summary>
        /// Updates Y-axis sprites positions and moves Y-axis buffers around
        /// </summary>
        /// <param name="screenBounds">the client screen bounds</param>
        protected void UpdateYAxis(Rectangle screenBounds)
        {
            // Return if Y-axis is locked
            if (lockYAxis) return;

            // Recalculate vertical (Y) positions and move sprites around the Y-axis 
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
                else if (positions[j].Y >= screenBounds.Height)
                {
                    positions[j].Y -= vLenScaled;
                }
            }
        }

        /// <summary>
        /// Sets the velocity and applies the velocity adjustment factor
        /// </summary>
        /// <param name="velocity">velocity to be set for the sprite's movement</param>
        private void SetVelocity(Vector2 velocity)
        {
            this.velocity = flipDirection ? -velocity : velocity;
            this.velocity *= velocityAdjustFactor;
        }

        // Velocity
        protected Vector2 velocity = Vector2.Zero;
        protected bool flipDirection;
        protected bool lockXAxis = false;
        protected bool lockYAxis = false;

        // Attributes
        public Vector2 Velocity
        {
            get { return velocity; }
            set { SetVelocity(value); }
        }

        public bool FlipDirection
        {
            get { return flipDirection; }
            set { flipDirection = value; }
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