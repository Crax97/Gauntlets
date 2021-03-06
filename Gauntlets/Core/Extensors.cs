﻿using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraxAwesomeEngine.Core
{
    public static class Extensors
    {

        public static void DrawEntity(this SpriteBatch batch, Entity entity, GameTime time)
        {
            if (entity.Enabled)
            {
                List<Sprite> sprites = entity.GetComponents<Sprite>();
                foreach (Sprite sprite in sprites)
                {
                    if (sprite.IsVisible)
                        batch.Draw(sprite.Texture, entity.Transform.Position,
                                   sprite.Source, Color.White, entity.Transform.Rotation, sprite.SpriteCenter,
                                    entity.Transform.LocalScale, sprite.RenderingEffect, sprite.RenderingOrder);
                }
            }
        }

    }
}
