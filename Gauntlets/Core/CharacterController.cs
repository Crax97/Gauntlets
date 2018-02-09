using CraxAwesomeEngine.Core.Physics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Core
{

    [Serializable]
    class CharacterController : IComponent
    {

        private Vector2 velocity;
        private AABBCollider playerCollider;
        private AnimatedSprite playerSprite;
        private Entity owner;
        private float lastDelta = 0.0f;
        private bool canJump = false;

        public float Mass { get; set; } = 40.0f;

        public Vector2 Gravity { get; set; }
        public Vector2 Extension {
            get
            {
                return playerCollider.Extension;
            }
            set
            {

                playerCollider.Extension = value;
            }
        }

        public object Clone()
        {
            return new CharacterController();
        }

        public void Destroy()
        {
            //throw new NotImplementedException();
        }

        public void Initialize(Entity owner)
        {
            playerSprite = owner.GetComponent<AnimatedSprite>();
            Vector2 extension = new Vector2(playerSprite.Width * 0.5f, playerSprite.Height);
            playerCollider = new AABBCollider(extension) ;
            playerCollider.IsStatic = false;
            //playerCollider.RemoveFromActiveColliders();
            playerCollider.Initialize(owner);

            this.owner = owner;


            Gravity = new Vector2(0, -9.8f);

        }
        
        public void Move(Vector2 translation)
        {

            playerCollider.UpdateBounds(owner.Transform.Position);
            AccountForGravity(ref translation);
            AccountForHorizontalVelocity(ref translation);
            AccountForVerticalVelocity(ref translation);
            owner.Transform.Translate(translation);

            SetProperAnimation(translation);
            
        }

        public void AccountForGravity(ref Vector2 translation)
        {
            translation -= velocity;
        }

        public void Reset()
        {
            velocity = Vector2.Zero;
        }

        public void AccountForHorizontalVelocity(ref Vector2 translation)
        {
            Vector2 xTranslation = new Vector2(translation.X, 0);
            playerCollider.UpdateBounds(owner.Transform.Position + xTranslation);
            Vector2? pushback = null;

            foreach (Collider collider in Collider.ActiveColliders())
            {

                if (playerCollider.StaticCollisionCheck(collider, out pushback) && !collider.Equals(playerCollider))
                {
                    translation += pushback.Value;
                }
            }

            playerCollider.UpdateBounds(owner.Transform.Position - xTranslation);

        }

        public void AccountForVerticalVelocity(ref Vector2 translation)
        {
            Vector2 yTranslation = new Vector2(0, translation.Y);
            playerCollider.UpdateBounds(owner.Transform.Position + yTranslation);
            Vector2? pushback = null;

            foreach (Collider collider in Collider.ActiveColliders())
            {

                if (playerCollider.StaticCollisionCheck(collider, out pushback) && !collider.Equals(playerCollider))
                {
                    translation += pushback.Value;
                    if(translation.Y <= 0)
                    {
                        canJump = true;
                        translation.Y = 0;
                    }
                    velocity.Y = 0;
                }
            }

            playerCollider.UpdateBounds(owner.Transform.Position);
        }

        public void Jump(float height)
        {
            if (canJump)
            {
                canJump = false;
                float yDisplacement = -Mass * height / 1.5f * lastDelta;
                velocity.Y = -yDisplacement;
            }
        }

        private void SetProperAnimation(Vector2 totalVelocity)
        {
            if (totalVelocity.X != 0 && canJump)
            {
                playerSprite.SetAnimation("running");
            }
            else if (!canJump)
            {
                playerSprite.SetAnimation("jump");
            }
            else
            {
                playerSprite.SetAnimation("idle");
            }
        }

        public void Update(float deltaTime, Entity parent)
        {
            velocity += Gravity * Mass / 10 * deltaTime;
            lastDelta = deltaTime;
        }
    }
}
