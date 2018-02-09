using System;
using Microsoft.Xna.Framework.Graphics;
using CraxAwesomeEngine.Core;
using Microsoft.Xna.Framework;
using System.Xml;

namespace CraxAwesomeEngine.Core.GUI
{
    [Serializable]
    class GUIImage : GUIElement, ICloneable
    {

        public GUIImage(Texture2D texture)
        {
            Sprite = new Sprite(texture, 0, 0, texture.Width, texture.Height, 0.0f);
        }

        public override object Clone()
        {
            GUIImage clone = new GUIImage(this.Sprite.Texture);
            return clone;
        }

        internal override void Draw(SpriteBatch batch, Entity parent)
        {
            Vector2 position = parent.Transform.getWorldPosition() + Transform.Position;
            batch.Draw(Sprite.Texture, position, Sprite.Source, Color.White, Transform.Rotation, Sprite.SpriteCenter, Transform.LocalScale, SpriteEffects.None, Sprite.RenderingOrder);
        }

    }
}
