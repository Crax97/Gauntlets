using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gauntlets.Core
{
    public static class Extensors
    {

        public static void SetupFromXmlNode(this Vector2 self, XmlAttribute attribute) {

            if(attribute != null)
            {

                if (self == null)
                    self = new Vector2();

                string[] vals = attribute.Value.Split(' ');
                self.X = float.Parse(vals[0]);
                self.Y = float.Parse(vals[1]);
            }
            else
            {
                self = Vector2.Zero;
            }
        }

        public static void SetupFromXmlNode(this Color color, XmlAttribute attribute) {
            if (attribute != null)
            {
                string[] vals = attribute.Value.Split(' ');
                color.R = byte.Parse(vals[0]);
                color.G = byte.Parse(vals[1]);
                color.B = byte.Parse(vals[2]);
                color.A = (vals[3] != null) ? byte.Parse(vals[3]) : (byte)255;
            }
            else
            {
                color = Color.White;
            }
        }

        public static void DrawEntity(this SpriteBatch batch, Entity entity, GameTime time)
        {
            if (entity.Enabled)
            {
                List<Sprite> sprites = entity.GetComponents<Sprite>();
                foreach (Sprite sprite in sprites)
                {
                    if (sprite.IsVisible)
                        batch.Draw(sprite.Texture, entity.Transform.PositionInCameraSpace,
                                   sprite.Source, Color.White, entity.Transform.Rotation, sprite.SpriteCenter,
                                    entity.Transform.LocalScale, SpriteEffects.None, sprite.RenderingOrder);
                }
            }
        }

    }
}
