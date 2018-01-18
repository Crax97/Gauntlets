using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Gauntlets.Core
{
    public class GUIBatch : SpriteBatch
    {
        public GUIBatch(GraphicsDevice device) : base(device)
        {
        }

        public void DrawGUIElement(GUIElement element) {
            Transform t = element.Transform;
            Sprite s = element.Sprite;
            Draw(s.Texture, t.PositionInCameraSpace, s.Source, Color.White, t.Rotation, s.SpriteCenter, t.LocalScale, SpriteEffects.None, s.RenderingOrder + 10);
        }

        public void DrawGUIElementsInWorld(World w) {
            
        }

    }
}
