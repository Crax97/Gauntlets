using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;

namespace Gauntlets.Core
{
	public static partial class SpriteBatchExtensor
	{
		/*SpriteBatch extension method for rendering Sprites*/
		public static void DrawSprite(this SpriteBatch batch, Sprite sprite, GameTime time)
		{
			if(sprite.IsVisible) 
			{
				batch.Draw(sprite.Texture, Vector2.Zero, null, sprite.Source, sprite.SpriteCenter, 0, Vector2.One, Color.White, SpriteEffects.None, sprite.RenderingOrder);
			}
		}
	}
    
    public class Sprite : IComponent
	{
		private SpriteEffect renderingEffect;
        private float renderingOrder;
        public Texture2D Texture { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public Rectangle Source { get; private set; }
        public SpriteEffect RenderingEffect { get; protected set; }
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

		/*You should NEVER call the empty constructor!*/
		public Sprite() {}

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
		public void Initialize() { }
		public void Destroy() { }

        public virtual void SetupFromXmlNode(XmlNode node, Game game)
        {
            XmlAttributeCollection attrib = node.Attributes;

            string textureName = attrib["texture"].Value;
            if (textureName == null) throw new ArgumentNullException("<Sprite ...></Sprite> must have at least a texture name!");

            this.Texture = game.Content.Load<Texture2D>(textureName);

            string widthStr = (attrib["width"] != null) ? attrib["width"].Value : null;
            string heightStr = (attrib["height"] != null) ? attrib["height"].Value : null;
            int width, height;

            //Schema: width [height] =
            //"full" => Texture.Width [Height]
            //"half" => Texture.Width/2 [Height]/2
            //"/n" => Texture.Width / n [Height / n]
            //"n" => n
            //nothing => full


            if (widthStr != null)
            {
                if (widthStr == "full")
                    width = Texture.Width;
                else if (widthStr == "half")
                    width = Texture.Width / 2;
                else if (widthStr[0] == '/')
                    width = Texture.Width / int.Parse(widthStr.Substring(1));
                else 
                    width = int.Parse(widthStr);
            }
            else
            {
                width = Texture.Width;
            }

            if (heightStr != null)
            {
                if (heightStr == "full")
                    height = Texture.Height;
                else if (heightStr == "half")
                    height = Texture.Height / 2;
                else if (heightStr[0] == '/')
                    height = Texture.Height / int.Parse(heightStr.Substring(1));
                else
                    height = int.Parse(heightStr);
            }
            else
            {
                height = Texture.Height;
            }
            int row = 0, column = 0;
            string rowStr = (attrib["row"] != null) ? attrib["row"].Value : null;
            if (rowStr != null)
            {
                row = int.Parse(rowStr);
            }
            else
            {
                row = 0;
            }

            string colStr = (attrib["column"] != null) ? attrib["column"].Value : null;
            if (colStr != null)
            {
                column = int.Parse(colStr);
            }

            renderingOrder = 0.0f;
            string roStr = (attrib["renderng_order"] != null) ? attrib["renderng_order"].Value : null;
            if (roStr != null)
            {
                renderingOrder = float.Parse(roStr);
            }

            SetSource(row, column, width, height);
            RenderingOrder = renderingOrder;

        }

        public object Clone()
        {
            return new Sprite(Texture, Source.X / Width, Source.Y / Height, Width, Height, RenderingOrder);
        }
    }
}
