using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;

namespace CraxAwesomeEngine.Core
{
	public static partial class SpriteBatchExtensor
	{
		/*SpriteBatch extension method for rendering Sprites*/
		public static void DrawSprite(this SpriteBatch batch, Sprite sprite, GameTime time)
		{
			if(sprite.IsVisible) 
			{
				batch.Draw(sprite.Texture, Vector2.Zero, null, sprite.Source, sprite.SpriteCenter, 0, Vector2.One, Color.White, sprite.RenderingEffect, sprite.RenderingOrder);
			}
		}
	}

    public class Sprite : IComponent
    {
        public SpriteEffects RenderingEffect {get; set;} = SpriteEffects.None;
        public Texture2D Texture { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public Rectangle Source { get; private set; }
        public Vector2 SpriteCenter { get; set; }
        public float RenderingOrder { get; set; }
        public bool IsVisible { get; set; } = true;
        public Vector2 Size
        {
            get
            {
                return new Vector2(Width, Height);
            }
        }

		public Sprite(Texture2D texture, int row, int column, int width, int height, float renderingOrder)
		{
            Texture = texture;
			SetSource(row, column, width, height);
            SpriteCenter = new Vector2(width / 2, height / 2);

            this.Width = width;
            this.Height = height;
            RenderingOrder = renderingOrder;

		}

		public void ToggleVisibility()
		{
			IsVisible = !IsVisible;
		}

        /// <summary>
        /// Changes this Rect's rendering rectangle.
        /// Kinda like changing the Texture's uvs
        /// </summary>
        /// <param name="row">Row.</param>
        /// <param name="column">Column.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
		public void SetSource(int row, int column, int width, int height)
		{
            Source = new Rectangle(column * width, row * height, width, height);
            Width = width;
            Height = height;
		}

		public virtual void Update(float delta, Entity e) {

        }
		public void Initialize(Entity owner) { }
		public void Destroy() { }


        public object Clone()
        {
            return new Sprite(Texture, Source.X / Width, Source.Y / Height, Width, Height, RenderingOrder);
        }
    }
}
