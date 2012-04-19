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
        /// <param name="xBuffer">X-axis buffer; this parameter defines how many sprites need
        /// to be aligned horizontally to create a transition smooth enough to avoid screen gaps;
        /// 2-3 is generally a good number</param>
        /// <param name="yBuffer">Y-axis buffer; this parameter defines how many sprites need
        /// to be aligned vertically to create a transition smooth enough to avoid screen gaps;
        /// note that the horizontal sprite already counts as 1 vertical sprite; 2-3 is generally
        /// a good number</param>
        /// <param name="scale">scale of the sprite to be drawn (1.0 = 100%)</param>
        /// <param name="flipDirection">flips the direction every time the velocity is updated; 
        /// e.g. player is moving at (x,y) = (5,2) and flip direction is set to true, then the
        /// sprite velocity is (x,y) = (-5,-2)</param>
        public ParallaxMovableSprite(Texture2D texture, Vector2 basePosition, float distanceFromCamera,
            Rectangle screenBounds, float viewingAngle = MathHelper.PiOver4, int xBuffer = 2, int yBuffer = 1,
            float scale = 1f, bool flipDirection = true)
            : base(texture, basePosition, distanceFromCamera, screenBounds, viewingAngle, xBuffer, yBuffer, scale)
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
        public override void Update(GameTime gameTime, Rectangle screenBounds, Vector2 velocity)
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

            // Checks if buffer sprite has to be reallocated to the LEFT (positive direction) or 
            // to the RIGHT (negative direction) when offscreen
            if (velocity.X > 0) // Positive direction (moving to the RIGHT)
            {
                // Check for each line of the sprite buffer matrix
                for (int m = 0; m < yBuffer; ++m)
                {
                    // Is the LAST sprite in this line offscreen ?
                    int lastColumnIndex = xBuffer - 1;
                    if (positions[m, lastColumnIndex].X >= screenBounds.Width)
                    {
                        // Recalculate LAST sprite's position to be reallocated to
                        // the position before the current FIRST sprite
                        positions[m, lastColumnIndex].X = positions[m, 0].X - wTextureScaled;
                        Vector2 lastPosition = positions[m, lastColumnIndex];

                        // Shift all sprites one index to the RIGHT so that the FIRST
                        // index (m, 0) is free
                        for (int n = xBuffer - 1; n > 0; --n)
                        {
                            positions[m, n] = positions[m, n - 1];
                        }

                        // Reallocate LAST sprite to the (now free) FIRST index
                        positions[m, 0] = lastPosition;
                    }
                }
            }
            else if (velocity.X < 0) // Negative direction (moving to the LEFT)
            {
                // Check for each line of the sprite buffer matrix
                for (int m = 0; m < yBuffer; ++m)
                {
                    // Is the FIRST sprite in this line offscreen ?
                    if (positions[m, 0].X + wTextureScaled <= 0)
                    {
                        // Recalculate FIRST sprite's position to be reallocated to
                        // the position after the current LAST sprite
                        positions[m, 0].X = positions[m, xBuffer - 1].X + wTextureScaled;
                        Vector2 firstPosition = positions[m, 0];

                        // Shift all sprites one index to the LEFT so that the LAST
                        // index (m, xBuffer-1) is free
                        for (int n = 0; n < xBuffer - 1; ++n)
                        {
                            positions[m, n] = positions[m, n + 1];
                        }

                        // Reallocate FIRST sprite to the (now free) LAST index
                        positions[m, xBuffer - 1] = firstPosition;
                    }
                }
            }

            // Apply to each line of the sprite buffer matrix
            for (int m = 0; m < yBuffer; ++m)
            {
                // Apply velocity's X-direction to the FIRST position (m, 0) of each line of the matrix and
                // reallocate each consecutive position (m, 1..n-1) at an offset from the FIRST position
                positions[m, 0].X += velocity.X;
                for (int n = 1; n < xBuffer; ++n)
                {
                    positions[m, n].X = positions[m, n - 1].X + wTextureScaled;
                }
            }
        }

        /// <summary>
        /// Updates Y-axis sprites positions and moves Y-axis buffers around
        /// </summary>
        /// <param name="screenBounds">the client screen bounds</param>
        protected void UpdateYAxis(Rectangle screenBounds)
        {
            //// Return if Y-axis is locked
            if (lockYAxis) return;

            // Checks if buffer sprite has to be reallocated to the TOP (downwards direction) or 
            // to the BOTTOM (upwards direction) when offscreen
            if (velocity.Y > 0) // Downwards direction
            {
                // Check for each column of the sprite buffer matrix
                for (int n = 0; n < xBuffer; ++n)
                {
                    // Is the BOTTOM sprite in this column offscreen ?
                    int bottomColumnIndex = yBuffer - 1;
                    if (positions[bottomColumnIndex, n].Y >= screenBounds.Height)
                    {
                        // Recalculate BOTTOM sprite's position to be reallocated to
                        // the position before the current TOP sprite
                        positions[bottomColumnIndex, n].Y = positions[0, n].Y - hTextureScaled;
                        Vector2 bottomPosition = positions[bottomColumnIndex, n];

                        // Shift all sprites so that the first index (0, n) is free
                        for (int m = yBuffer - 1; m > 0; --m)
                        {
                            positions[m, n] = positions[m - 1, n];
                        }

                        // Reallocate BOTTOM sprite to the (now free) TOP index
                        positions[0, n] = bottomPosition;
                    }
                }
            }
            else if (velocity.Y < 0) // Upwards direction
            {
                // Check for each column of the sprite buffer matrix
                for (int n = 0; n < xBuffer; ++n)
                {
                    // Is the TOP sprite in this column offscreen ?
                    if (positions[0, n].Y + hTextureScaled <= 0)
                    {
                        // Recalculate TOP sprite's position to be reallocated to the position 
                        // after the current BOTTOM sprite
                        positions[0, n].Y = positions[yBuffer - 1, n].Y + hTextureScaled;
                        Vector2 topPosition = positions[0, n];

                        // Shift all sprites so that the BOTTOM index (yBuffer-1, n) is free
                        for (int m = 0; m < yBuffer - 1; ++m)
                        {
                            positions[m, n] = positions[m + 1, n];
                        }

                        // Reallocate TOP sprite to the (now free) BOTTOM index
                        positions[yBuffer - 1, n] = topPosition;
                    }
                }
            }

            // Apply to each column of the sprite buffer matrix
            for (int n = 0; n < xBuffer; ++n)
            {
                // Apply velocity's Y-direction to the TOP position (0, n) of each column of the matrix and
                // reallocate each consecutive position (1..m-1, n) at an offset from the TOP position
                positions[0, n].Y += velocity.Y;
                for (int m = 1; m < yBuffer; ++m)
                {
                    positions[m, n].Y = positions[m - 1, n].Y + hTextureScaled;
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

        /// <summary>
        /// Flips velocity direction
        /// </summary>
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