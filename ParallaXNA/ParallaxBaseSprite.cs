using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

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
        /// <param name="bounds">screen/client bounds</param>
        /// <param name="hBuffer">X-axis buffer; this parameter defines how many sprites need
        /// to be aligned horizontally to create a transition smooth enough to avoid screen gaps;
        /// 2-3 is generally a good number</param>
        /// <param name="vBuffer">Y-axis buffer; this parameter defines how many sprites need
        /// to be aligned vertically to create a transition smooth enough to avoid screen gaps;
        /// note that the horizontal sprite already counts as 1 vertical sprite; 2-3 is generally
        /// a good number</param>
        /// <param name="scale">scale of the sprite to be drawn (1.0 = 100%)</param>
        /// <param name="layerDepth">layer depth at which the sprite will be drawn</param>
        public ParallaxBaseSprite(Texture2D texture, Vector2 basePosition, Rectangle bounds,
            int hBuffer = 2, int vBuffer = 1, float scale = 1f, float layerDepth = 0)
        {
            this.enabled = true; // default enabled
            this.hBuffer = hBuffer;
            this.vBuffer = vBuffer;
            this.texture = texture;
            this.scale = scale;
            this.basePosition = basePosition;
            this.bounds = bounds;
            this.layerDepth = layerDepth;

            // hBuffer and vBuffer minimum value is 1
            Trace.Assert(hBuffer > 0 && vBuffer > 0);

            // Apply scale to fields
            UpdateScales();

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
        protected virtual void UpdateScales()
        {
            wTextureScaled = texture.Width * scale;
            hTextureScaled = texture.Height * scale;
            hLenScaled = hBuffer * wTextureScaled;
            vLenScaled = vBuffer * hTextureScaled;
        }

        /// <summary>
        /// Called at each update cycle of the game
        /// </summary>
        /// <param name="gameTime">GameTime object; usually retrieved from 
        /// the Game instance</param>
        public virtual void Update(GameTime gameTime) { }

        // Fields
        protected bool enabled = true;
        protected bool visible = true;
        protected Texture2D texture;
        protected Vector2 basePosition;
        protected Vector2[] positions;
        protected Rectangle bounds;

        protected float scale;
        protected int hBuffer;
        protected int vBuffer;
        private float layerDepth;

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

        public Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
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
    }
}