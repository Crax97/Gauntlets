using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CraxAwesomeEngine.Core.Physics
{
    class CharacterCollider : AABBCollider
    {

        public CharacterCollider(Vector2 extension) : base(extension)
        {
            IsStatic = false;
        }

        public bool CheckCollisionWithVelocity(Vector2 velocity, uint samples = 8)
        {
            Vector2 finalPosition = Owner.Transform.Position + velocity;
            float samplingDelta = 1.0f / samples;
            AABBCollider pushedCollider = new AABBCollider(Extension);
            pushedCollider.Initialize(Owner);
            for (float f = 0.0f; f <= 1.0f; f += samplingDelta)
            {
                Vector2 lerpedPosition = Vector2.Lerp(Owner.Transform.Position, finalPosition, f);
                pushedCollider.UpdateBounds(lerpedPosition);
                foreach (Collider collider in Collider.colliders)
                {
                    Vector2? MTV;
                    if (pushedCollider.StaticCollisionCheck(collider, out MTV))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Initialize(Entity owner)
        {
            base.Initialize(owner);

        }

        public override object Clone()
        {
            CharacterCollider clone = new CharacterCollider(Extension);
            return clone;           
        }

    }
}
