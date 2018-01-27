using System;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace CraxEngine.Core
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

		public AnimatedSprite(Texture2D texture, List<AnimationFrame> frames, int width, int height, float renderingOrder)
			: base(texture, frames[0].row, frames[0].column, width, height, renderingOrder)
		{
			this.frames = frames;
			currentFrame = frames[0];
		}

        public override void Update(float delta, Entity e)
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

		public new void Initialize(Entity owner) { }
		public new void Destroy() { }

        public new object Clone() {

            List<AnimationFrame> copiedFrames = new List<AnimationFrame>(frames);
            return new AnimatedSprite(Texture, copiedFrames, Width, Height, RenderingOrder);

        }

	}
}
