using System;
using System.Xml;
using CraxAwesomeEngine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CraxAwesomeEngine.Core.GUI { 
    /// <summary>
    /// Abstracts a GUI button with two callbacks:
    /// On Click and On Release.
    /// </summary>
    /// 

public class GUIButton : GUIElement, ICloneable
	{

		public delegate void ButtonCallback();

		private ButtonCallback onClick;
		private ButtonCallback onRelease;

		private bool hadPrevClicked = false;
        private Vector2 origSize = Vector2.Zero;
        private Vector2 offset = Vector2.Zero;
        private Vector2 extension = Vector2.Zero;

        /// <summary>
        /// How much big should be the rect responsible
        /// for the Button behaviour.
        /// </summary>
        /// <value>The rect extension.</value>
        public Vector2 Extension { 
            get{
                return extension;   
            }
            set
            {
                extension = value;
                offset = (origSize - extension) * 0.5f;
            }
        }

		///<summary>
		///Takes a 2x2 texture alpha.
		///Texture 0x0 is the IDLE State, 0x1 is MOUSE_HOVER state and 1x0 is the PRESSED state
		///</summary>
		///<param>image</param> The texture atlas
		public GUIButton(Texture2D image) : base()
		{
            Sprite = new Sprite(image, 0, 0, image.Width / 2, image.Height / 2, 0);
            origSize = Extension = new Vector2(image.Width / 2, image.Height / 2);

        }

		public void AddOnClickCallback(ButtonCallback click)
		{
			onClick += click;
		}

		public void AddOnReleaseCallback(ButtonCallback release)
		{
			onRelease += release;
		}

		public override void Update(float deltaTime, Entity parent)
		{

            //Math used: I multiply the SpriteCenter by LocalScale because (of course) the more i scale, the more the center moves. 
            //We have to accout for extension differences as well.
            //Same goes for the buttonSprize's size, and that's more obvious 
            Vector2 rectPosition = (parent.Transform.LocalPosition + Transform.Position - (Sprite.SpriteCenter * Transform.LocalScale) + (offset * Transform.LocalScale));
            Rectangle rect = new Rectangle(rectPosition.ToPoint(), (Extension * Transform.LocalScale).ToPoint());
			MouseState mouseState = Mouse.GetState();

			//Pretty explanatory i hope. But i already know i will forget this stuff in one day
			if (rect.Contains(mouseState.Position))
			{
                Sprite.SetSource(0, 1, Sprite.Width, Sprite.Height);
				if (mouseState.LeftButton == ButtonState.Pressed)
				{
                    Sprite.SetSource(1, 0, Sprite.Width, Sprite.Height);
					if (!hadPrevClicked) {
						if(onClick != null) onClick();
							
						hadPrevClicked = true;
					}
				}
				else if (hadPrevClicked)
				{

					hadPrevClicked = false;
					if (onRelease != null) onRelease();
				}
			}
			else
            {
                //Reset the button without calling the onRelease callback.
                //Users tend to move the cursor outside the button if they clicked by mistake
                //or they don't want the onRelease action
                if (hadPrevClicked)
				{
					hadPrevClicked = false;
				}
                Sprite.SetSource(0, 0, Sprite.Width, Sprite.Height);
			}

        }

       

        internal override void Draw(SpriteBatch batch, Entity parent)
        {
            batch.Draw(Sprite.Texture, parent.Transform.LocalPosition + Transform.Position, Sprite.Source, Color, Transform.Rotation,
                                  Sprite.SpriteCenter, Transform.LocalScale, SpriteEffects.None, Sprite.RenderingOrder);
        }

        public override object Clone() {

            Texture2D texture = Sprite.Texture;
            GUIButton btn = new GUIButton(texture)
            {
                Extension = this.Extension
            };
            btn.Sprite.RenderingOrder = Sprite.RenderingOrder;
            return btn;

        }
	}
}
