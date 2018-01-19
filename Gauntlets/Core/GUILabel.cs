using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;

namespace Gauntlets.Core
{
    /// <summary>
    /// Abstracts a GUI Label, with settable SpriteFont, 
    /// a center and a rendering depth.
    /// </summary>
    class GUILabel : GUIElement, ICloneable
    {

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

        public override void SetupFromXmlNode(XmlNode node, Game game) {
            
            XmlAttributeCollection attributes = node.Attributes;
            _label = (attributes["label"] != null) ? attributes["label"].Value.Replace("\\n", "\n") : "";
            Font = (attributes["font"] != null && attributes["font"].Value != "default") ? game.Content.Load<SpriteFont>(attributes["font"].Value) : defaultSpriteFont;
            Depth = (attributes["rendering_depth"] != null) ? float.Parse(attributes["rendering_depth"].Value) : 0.0f;
            Color.SetupFromXmlNode(attributes["color"]);
            Center.SetupFromXmlNode(attributes["center"]);

            XmlNode possibleTransform = node.ChildNodes[0];
            if (possibleTransform != null)
                Transform.SetupFromXmlNode(possibleTransform, game);

        }

        public override object Clone() {
            GUILabel cpy = new GUILabel(Label, Depth, Font);
            cpy.Center = this.Center;
            cpy.Transform = this.Transform.Clone() as Transform;
            return cpy;
        }

        internal override void Draw(SpriteBatch batch, Entity parent)
        {
            Vector2 position = parent.Transform.LocalPosition + this.Transform.PositionInCameraSpace - offset;
            batch.DrawString(Font, Label, position, Color, Transform.Rotation, Center, Transform.LocalScale, SpriteEffects.None, Depth);
        }

    }
}
