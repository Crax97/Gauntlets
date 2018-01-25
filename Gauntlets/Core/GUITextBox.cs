using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gauntlets.Core
{
    class GUITextBox : GUIElement
    {

        private GUILabel label;
        private Vector2 _offset;
        public Vector2 Offset {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                SetWrappedText(label.Label);
            }
        }

        public GUITextBox() {

        }

        public GUITextBox(Texture2D texture, string initialText) : base() {
            Sprite = new Sprite(texture, 0, 0, texture.Width, texture.Height, 0.0f);
            label = new GUILabel("");
            _offset = Vector2.Zero;

            SetWrappedText(initialText);
        }

        private void SetWrappedText(string text) {

            label.Label = "";

            var glyphs = label.Font.GetGlyphs();
            float width = 0;

            string parsedString = "";

            foreach(char c in text) {
                width +=(glyphs.ContainsKey(c)) ? glyphs[c].Width : 0;
                if ( width + (Offset.X * 1.5f) > Sprite.Width)
                {
                    parsedString += '\n';
                    width = 0;
                }
                if(!char.IsControl(c))
                    parsedString += c;
            }

            label.Label = parsedString;

        }

        internal override void Draw(SpriteBatch batch, Entity parent) {
            Vector2 textBoxPosition = parent.Transform.getWorldPosition() + Transform.PositionInCameraSpace;
            Vector2 labelPosition = textBoxPosition + Offset - Sprite.Size * 0.5f;
            if( Sprite != null ) batch.Draw(Sprite.Texture, textBoxPosition, Sprite.Source, Color.White, Transform.Rotation, Sprite.SpriteCenter, Transform.LocalScale, SpriteEffects.None, Sprite.RenderingOrder );
            batch.DrawString(label.Font, label.Label, labelPosition, label.Color, Transform.Rotation, label.Center, Transform.LocalScale, SpriteEffects.None, Sprite.RenderingOrder + 0.1f);
        }

        /// <summary>
        /// Setups this component from an XML node.
        /// Schema:
        /// [texture="texture name"]    | Setups the GUITextBox's sprite component with this sprite.
        /// [label="string"]            | Setups this GUITextBox's label. If none is present, the default "Text!" string is used.
        /// [offset="n n"]              | Setups this GUITextBox's label offset. If none is present, the default Vector2.Zero value is used.
        /// [font="font name"]          | Setups this GUITextBox's <see cref="SpriteFont"/>. If not present, GUILabel.DefaultSpriteFont will be used.
        /// If a <see cref="Transform"/> child node is present, the Transform property of this component will be setup accordingly
        /// </summary>
        /// <param name="node"></param>
        /// <param name="game"></param>
        public override void SetupFromXmlNode(XmlNode node, Game game)
        {
            base.SetupFromXmlNode(node, game);
            XmlAttributeCollection attributes = node.Attributes;
            Texture2D texture = game.Content.Load<Texture2D>(attributes["texture"].Value);
            Sprite = new Sprite(texture, 0, 0, texture.Width, texture.Height, 0.0f);
            SpriteFont font = (attributes["font"] != null) ? game.Content.Load<SpriteFont>(Path.Combine("Fonts", attributes["font"].Value)) : GUILabel.DefaultSpriteFont;

            label = (attributes["label"] != null) ? new GUILabel(attributes["label"].Value, 0.0f) : new GUILabel("Text!", 0.0f);
            Offset.SetupFromXmlNode(attributes["offset"]);
            Transform = new Transform();
            Transform.SetupFromXmlNode(node["transform"], game);
        }

        public override object Clone() {

            GUITextBox cpy = new GUITextBox(Sprite.Texture, label.Label)
            {
                Offset = this.Offset
            };
            return cpy;

        }

    }
}
