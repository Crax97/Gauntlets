using System;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CraxAwesomeEngine.Core.Debugging;

/// <summary>
/// Part of Crax's Awesome Engine.
/// </summary>
namespace CraxAwesomeEngine.Core
{

    /// <summary>
    /// An animated sprite, based on <see cref="Sprite"/>,
    /// it can be attached to an Entity since it's an <see cref="IComponent"></see>
    /// </summary>
    [Serializable]
    public class AnimatedSprite : Sprite, IComponent
	{

        [Serializable]
        public struct AnimationFrame
		{
			public int column;
			public int row;
			public float duration;

			public AnimationFrame(int c, int r, float f)
			{
				row = r;
				column = c;
				duration = f;
			}
		}

        [Serializable]
        public struct Animation
        {
            public List<AnimationFrame> frames;
            public bool canLoop;
            public string name;

            public Animation(string n, bool l = true)
            {
                frames = new List<AnimationFrame>();
                name = n;
                canLoop = l;
            }
        }

		private List< Animation > animations;
		private float currentTime = 0.0f;
		private Animation currentAnimation;
        private AnimationFrame currentFrame;
		private int index = 0;

		public AnimatedSprite(Texture2D texture, List<Animation> animationsList, int width, int height, float renderingOrder)
			: base(texture, 0, 0, width, height, renderingOrder)
		{

            animations = animationsList;
			currentAnimation = animations[0];
            currentFrame = currentAnimation.frames[0];
		}

        public override void Update(float delta, Entity e)
        {
            if (currentFrame.duration != 0.0f)
            {
                currentTime += delta;
                if (currentTime > currentFrame.duration)
                {
                    index++;
                    if (index >= currentAnimation.frames.Count)
                    {
                        if (currentAnimation.canLoop) index = 0;
                        else index = currentAnimation.frames.Count - 1;
                    }
                    currentFrame = currentAnimation.frames[index];
                    SetSource(currentFrame.row, currentFrame.column, Width, Height);
                    currentTime = 0.0f;
                }
            }

        }

		public new void Initialize(Entity owner) { }
		public new void Destroy() { }

        /// <summary>
        /// Sets this current sprite's animation,
        /// logs to the Debug Console if it can't find the animation
        /// </summary>
        /// <param name="animationName">Name of the animation to set</param>
        public void SetAnimation(string animationName)
        {
            if (GetCurrentAnimationName() != animationName)
            {
                foreach (Animation anim in animations)
                {
                    if (anim.name == animationName)
                    {
                        currentAnimation = anim;
                        SetAnimationFrame(0);
                        return;
                    }
                }
            Debug.Log("Could not find animation {0} from AnimatedSprite whose texture is {1}!", animationName, Texture.Name);
            }
        }

        /// <summary>
        /// Sets the current animation frame,
        /// logs to the Debug Console if it can't find the frame
        /// </summary>
        /// <param name="frame">Index of the frame to set, starts from 0</param>
        public void SetAnimationFrame(int frame)
        {
            if(frame >= currentAnimation.frames.Count)
            {
                Debug.Log("Can't use frame index {0} there are less frames in the current animation!", frame);
                return;
            }

            currentFrame = currentAnimation.frames[frame];
            currentTime = 0.0f;
            index = frame;
            SetSource(currentFrame.row, currentFrame.column, Width, Height);
        }

        /// <summary>
        /// Returns the current running frame
        /// </summary>
        /// <returns></returns>
        public int GetCurrentFrame()
        {
            return index;
        }

        /// <summary>
        /// Returns the current playing animation
        /// </summary>
        /// <returns></returns>
        public string GetCurrentAnimationName()
        {
            return currentAnimation.name;
        }

        public new object Clone() {

            List<Animation> copiedAnimations = new List<Animation>(animations);
            return new AnimatedSprite(Texture, copiedAnimations, Width, Height, RenderingOrder);

        }

	}
}
