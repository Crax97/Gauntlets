﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;
using System.IO;

namespace Gauntlets.Core
{
    /// <summary>
    /// Abstracts a GUI Label, with settable SpriteFont, 
    /// a center and a rendering depth.
    /// </summary>
    /// 
    
    public class GUILabel : GUIElement, ICloneable
    {

        private static bool hasBeenInitialized = false;
        public static SpriteFont DefaultSpriteFont { get; private set; } = null;
        
        public string Label { get; set; }
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
            Font = (font != null) ? font : DefaultSpriteFont;
            Sprite = new Sprite(Font.Texture, 0, 0, Font.Texture.Width, Font.Texture.Height, 1.0f);
            Label = label;
            Depth = depth;
        }

        public static void Initialize(Game game) {
            if (hasBeenInitialized)
                throw new NotSupportedException("GUILabel has already been initialized!");

            DefaultSpriteFont = game.Content.Load<SpriteFont>(Path.Combine("Fonts", "DefaultFont"));

        }

        public override void SetupFromXmlNode(XmlNode node, Game game) {
            
            XmlAttributeCollection attributes = node.Attributes;
            Label = (attributes["label"] != null) ? attributes["label"].Value.Replace("\\n", "\n") : "";
            Font = (attributes["font"] != null && attributes["font"].Value != "default") ? game.Content.Load<SpriteFont>(attributes["font"].Value) : DefaultSpriteFont;
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
            Vector2 position = parent.Transform.LocalPosition + this.Transform.PositionInCameraSpace;
            batch.DrawString(Font, Label, position, Color, Transform.Rotation, Center, Transform.LocalScale, SpriteEffects.None, Depth);
        }

    }
}
