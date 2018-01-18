using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gauntlets.Core
{
    /// <summary>
    /// Abstracts a GUI Label, with settable SpriteFont, 
    /// a center and a rendering depth.
    /// </summary>
    class GUILabel : GUIElement, ICloneable
    {
        // TODO: Implement SetupFromXMLNode(..)

        private static bool hasBeenInitialized = false;
        private static SpriteFont defaultSpriteFont = null;

        // The offset is the center of the current
        // rendered label. 
        private Vector2 offset;
        private string _label;
        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
                offset = Font.MeasureString(_label) * 0.5f;
            }
        }
        public SpriteFont Font { get; set; }
        public Vector2 Center { get; set; }
        public float Depth { get; set; } 
        /// <summary>
        /// Basic constructor.
        /// </summary>
        public GUILabel() : this("GUI Label") { }

        /// <summary>
        /// Creates a GUILabel with a text, an optional rendering depth which defaults to 0
        /// and an optional SpriteFont wich defaults to the Oswald font.
        /// Oswald font: https://fonts.google.com/specimen/Oswald
        /// </summary>
        /// <param name="label">The label text.</param>
        /// <param name="depth">The layer depth.</param>
        /// <param name="font">The rendering font.</param>
        public GUILabel(string label, float depth = 0.0f, SpriteFont font = null) :base() {
            Font = (font != null) ? font : defaultSpriteFont;
            Label = label;
            Depth = depth;
        }

        public static void Initialize(Game game) {
            if (hasBeenInitialized)
                throw new NotSupportedException("GUILabel has already been initialized!");

            defaultSpriteFont = game.Content.Load<SpriteFont>("Fonts/DefaultFont");

        }

        internal override void Draw(SpriteBatch batch, Entity parent)
        {
            batch.DrawString(Font, Label, parent.Transform.LocalPosition + this.Transform.PositionInCameraSpace - offset + Center, Color);
        }

    }
}
