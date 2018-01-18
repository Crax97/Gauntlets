﻿using System;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Gauntlets.Core
{
	public class AnimatedSprite : Sprite, IComponent
	{

		public struct AnimationFrame
		{
			public int column;
			public int row;
			public float timeToNextFrame;

			public AnimationFrame(int c, int r, float f)
			{
				row = r;
				column = c;
				timeToNextFrame = f;
			}
		}

		private List<AnimationFrame> frames;
		private float currentTime = 0.0f;
		private AnimationFrame currentFrame;
		private int index = 0;

		/*You should NEVER call the empty costructor!*/
		public AnimatedSprite() : base() { }

		public AnimatedSprite(Texture2D texture, List<AnimationFrame> frames, int width, int height, float renderingOrder)
			: base(texture, frames[0].row, frames[0].column, width, height, renderingOrder)
		{
			this.frames = frames;
			currentFrame = frames[0];
		}

        public new Component getComponent() {
            return Component.SPRITE;
        }

		public new string getComponentName()
		{
			return "Animated Sprite";
		}

        public override void Update(float delta)
		{
			currentTime += delta;
			if (currentTime > currentFrame.timeToNextFrame)
			{
				index++;
				if (index >= frames.Count) index = 0;
				currentFrame = frames[index];
				SetSource(currentFrame.row, currentFrame.column, Width, Height);
				currentTime = 0.0f;
			}

        }

        public override void SetupFromXmlNode(XmlNode node, Game game) 
        {

            XmlAttributeCollection attrib = node.Attributes;

            string textureName = attrib["texture"].Value;
            if (textureName == null) throw new ArgumentNullException("<AnimatedSprite ...></AnimatedSprite> must have at least a texture name!");

            this.Texture = game.Content.Load<Texture2D>(textureName);

            string widthStr = (attrib["width"] != null) ? attrib["width"].Value : null;
            string heightStr = (attrib["height"] != null) ? attrib["height"].Value : null;
            int width, height;
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

            uint renderingOrder = 0;
            string roStr = (attrib["renderng_order"] != null) ? attrib["renderng_order"].Value : null;
            if (roStr != null)
            {
                renderingOrder = uint.Parse(roStr);
            }

            //Parsing frames
            XmlNodeList framesNodes = node.ChildNodes;
            if (framesNodes.Count == 0) throw new ArgumentException("An animated frame must have at least one <Frame...></Frame> element!");
          
            List<AnimationFrame> frames = new List<AnimationFrame>(framesNodes.Count);
             foreach(XmlNode frameNode in framesNodes) {
                XmlAttributeCollection collection = frameNode.Attributes;

                //All these three attributes MUST be present!
                int row = int.Parse(collection["row"].Value);
                int column = int.Parse(collection["column"].Value);
                float timeBetween = float.Parse(collection["time"].Value);

                frames.Add(new AnimationFrame(column, row, timeBetween));
            }

            this.frames = frames;
            currentFrame = frames[0];

            SetSource(frames[0].row, frames[0].column, width, height);
            RenderingOrder = renderingOrder;
        }

		public new void Initialize() { }
		public new void Destroy() { }

        public new object Clone() {

            List<AnimationFrame> copiedFrames = new List<AnimationFrame>(frames);
            return new AnimatedSprite(Texture, copiedFrames, Width, Height, RenderingOrder);

        }

	}
}
