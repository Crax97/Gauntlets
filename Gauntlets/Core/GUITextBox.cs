using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            batch.Draw(Sprite.Texture, textBoxPosition, Sprite.Source, Color.White, Transform.Rotation, Sprite.SpriteCenter, Transform.LocalScale, SpriteEffects.None, Sprite.RenderingOrder );
            batch.DrawString(label.Font, label.Label, labelPosition, label.Color, Transform.Rotation, label.Center, Transform.LocalScale, SpriteEffects.None, Sprite.RenderingOrder + 0.1f);
        }

    }
}
