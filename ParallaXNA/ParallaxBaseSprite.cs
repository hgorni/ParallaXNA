using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System;

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
    /// Parallax base sprite class. This class implements most of the required functionality
    /// and may be used as a static sprite or background
    /// 
    /// Author: Henrique Gorni (gorni.henrique@gmail.com)
    /// URL: http://www.demiurgo.com.br
    /// </summary>
    public class ParallaxBaseSprite
    {
        /// <summary>
        /// Initializes the base sprite. Avoid using sprites that are smaller than the visible
        /// screen so that less buffer sprites are necessary and, therefore, less overhead is
        /// necessary to recalculate their positions.
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
        public ParallaxBaseSprite(Texture2D texture, Vector2 basePosition, float distanceFromCamera,
            Rectangle screenBounds, float viewingAngle = MathHelper.PiOver4, int hBuffer = 2,
            int vBuffer = 1, float scale = 1f)
        {
            this.enabled = true; // default enabled
            this.hBuffer = hBuffer;
            this.vBuffer = vBuffer;
            this.texture = texture;
            this.scale = scale;
            this.basePosition = basePosition;
            this.distanceFromCamera = distanceFromCamera;
            this.screenBounds = screenBounds;
            this.viewingAngle = viewingAngle;

            // hBuffer and vBuffer minimum value is 1
            Trace.Assert(hBuffer > 0 && vBuffer > 0);

            // distanceFromCamera must be greater than 0
            Trace.Assert(distanceFromCamera > 0);

            // Apply scale to fields
            UpdateScales(screenBounds);

            InitializeBuffers();
        }

        /// <summary>
        /// Initializes sprite buffers
        /// </summary>
        protected virtual void InitializeBuffers()
        {
            // Initialize buffer with first sprite positioned at base position
            positions = new Vector2[hBuffer * vBuffer];
            this.positions[0] = this.basePosition;

            // Add each additional sprite positioned at an offset from the sprite preceding it
            for (int h = 1; h < hBuffer; ++h)
            {
                this.positions[h] = new Vector2(positions[h - 1].X + wTextureScaled, positions[h - 1].Y);
            }

            // Add vertical buffer sprites relative to the horizontal sprite
            // Note: each horizontal sprite count as one vertical sprite each; therefore for each horizontal sprite
            // vBuffer-1 sprites are added
            int vIdx = hBuffer;
            for (int h = 0; h < hBuffer; ++h)
            {
                Vector2 hSprite = positions[h];

                for (int v = 1; v < vBuffer; ++v)
                {
                    positions[vIdx++] = new Vector2(hSprite.X, hSprite.Y + hTextureScaled * v);
                }
            }
        }

        /// <summary>
        /// Gets the positions at which the sprite will be drawn on the screen
        /// </summary>
        /// <returns>list of positions to be drawn</returns>
        public Vector2[] GetPositions()
        {
            return visible ? this.positions : new Vector2[0];
        }

        /// <summary>
        /// Updates the scales of metrics according to the scale parameter
        /// </summary>
        /// <param name="screenBounds">the client screen bounds</param>
        protected virtual void UpdateScales(Rectangle screenBounds)
        {
            // Update screen bounds
            this.screenBounds = screenBounds;

            // Update scales
            wTextureScaled = texture.Width * scale;
            hTextureScaled = texture.Height * scale;
            hLenScaled = hBuffer * wTextureScaled;
            vLenScaled = vBuffer * hTextureScaled;

            // Calculate layer depth
            CalculateLayerDepth();

            // Keeps the calculation proportional across all screen sizes
            float widthScreenFactor = 1f / screenBounds.Width;
            float heightScreenFactor = 1f / screenBounds.Height;

            // opposite = tan(theta) / 2 * adjacent
            float triangleOpposite = (float)Math.Tan(viewingAngle) / 2 * distanceFromCamera;

            // Calculate the proportion of velocity for each axis
            float xAxisFactor = 1 / (triangleOpposite * widthScreenFactor * 2);
            float yAxisFactor = 1 / (triangleOpposite * heightScreenFactor * 2);
            this.velocityAdjustFactor = new Vector2(xAxisFactor, yAxisFactor);
        }

        /// <summary>
        /// Set layer depth proportional to the distance from the camera
        /// </summary>
        private void CalculateLayerDepth()
        {
            if (spriteSortMode == SpriteSortMode.FrontToBack)
                this.layerDepth = 1 / distanceFromCamera;
            else
                this.layerDepth = 1 - 1 / distanceFromCamera;
        }

        /// <summary>
        /// Called at each update cycle of the game
        /// </summary>
        /// <param name="gameTime">GameTime object; usually retrieved from 
        /// the Game instance</param>
        /// <param name="screenBounds">the client screen bounds</param>
        public virtual void Update(GameTime gameTime, Rectangle screenBounds)
        {
            // Refactor metrics when screen size changes
            if (this.screenBounds != screenBounds)
                UpdateScales(screenBounds);
        }

        // Fields
        protected bool enabled = true;
        protected bool visible = true;
        protected Texture2D texture;
        protected Vector2 basePosition;
        protected Vector2[] positions;

        protected float scale;
        protected int hBuffer;
        protected int vBuffer;
        private float layerDepth;
        private float distanceFromCamera;
        private Rectangle screenBounds;
        private float viewingAngle;
        private SpriteSortMode spriteSortMode = SpriteSortMode.BackToFront;

        // Defines how much of the actual camera/player velocity
        // is used proportional to the distance of the layer
        // from the camera into the horizon
        protected Vector2 velocityAdjustFactor;

        // Scaled values
        protected float hLenScaled;
        protected float vLenScaled;
        protected float wTextureScaled;
        protected float hTextureScaled;

        // Attributes
        public Texture2D Texture
        {
            get { return texture; }
        }

        public Vector2 BasePosition
        {
            get { return basePosition; }
        }

        public float Scale
        {
            get { return scale; }
        }

        /// <summary>
        /// Sets the Parallax sprite enabled/disabled. If disabled, the sprite can 
        /// still be drawn but any updates will be ignored.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Sets the visibility of the sprite. If set to false, the sprite will not
        /// return positions to be drawn.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public float LayerDepth
        {
            get { return layerDepth; }
        }

        public float DistanceFromCamera
        {
            get { return distanceFromCamera; }
        }

        public float ViewingAngle
        {
            get { return viewingAngle; }
        }

        /// <summary>
        /// Sets the SpriteSortMode. This affects how the sprites draw order
        /// is sorted according to their layer depth.
        /// </summary>
        public SpriteSortMode @SpriteSortMode
        {
            get { return spriteSortMode; }
            set
            {
                spriteSortMode = value;
                CalculateLayerDepth();
            }
        }
    }
}