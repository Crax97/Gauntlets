using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Core.Physics
{

    public enum ColliderType
    {
        AABB,
        CIRCLE,
        SAT
    }

    public abstract class Collider : IComponent
    {
        protected static List<Collider> colliders = new List<Collider>();
        private static uint instancesCount = 0;

        public Entity Owner { get; private set; } = null;
        public bool IsStatic { get; set; } = false;
        public Action<Collider> OnCollision { get; set; } = null;
        public bool isColliding = false;
        public abstract List<Shape> GetShapes();
        public abstract ColliderType GetColliderType();

        protected Vector2 deltaPosition;
        private uint instanceId;
        private Vector2 positionLastFrame;

        /// <summary>
        /// Checks collisions for objects
        /// </summary>
        public static void CalculateCollisions()
        {
            foreach (Collider collider in colliders)
            {
                    foreach (Collider other in colliders)
                    {
                    if (other != collider)
                    {
                        Vector2? pushback;
                        collider.StaticCollisionCheck(other, out pushback);
                        if (pushback != null)
                        {
                            if (!collider.IsStatic) collider.Owner.Transform.Translate(pushback.Value);
                            collider.OnCollision?.Invoke(other);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if two shapes are colliding using
        /// the Separating Axis Theorem
        /// </summary>
        /// <param name="other"></param>
        /// <returns>false if no collision is detected, true otherwise</returns>
        public bool StaticCollisionCheck(Collider other, out Vector2? MTV)
        {

            isColliding = false;
            MTV = null;
            List<Shape> myShapes = GetShapes();
            List<Shape> otherShapes = other.GetShapes();

            foreach (Shape myShape in myShapes)
            {
                foreach (Shape otherShape in otherShapes)
                {
                    Vector2? shapeTV = null;
                    if (myShape.IsCollidingWithOtherShape(otherShape, out shapeTV, this.Owner.Transform.Position, other.Owner.Transform.Position)) 
                    {
                        if (MTV != null)
                            MTV += shapeTV;
                        else
                            MTV = shapeTV;
                    }
                }                   

            }
            return MTV != null;
        }

        public virtual void Initialize(Entity owner)
        {
            colliders.Add(this);
            this.Owner = owner;
        }
       
        public Collider()
        {
            instanceId = instancesCount;
            instancesCount++;
        }

        public override bool Equals(object obj)
        {
            return (obj is Collider && (obj as Collider).instanceId == this.instanceId);
        }

        public virtual void Update(float deltaTime, Entity parent)
        {
            deltaPosition = parent.Transform.Position - positionLastFrame;
            positionLastFrame = parent.Transform.Position;
        }


        public void Destroy()
        {
            colliders.Remove(this);
        }

        public bool IsColliding()
        {
            return isColliding;
        }

        public void RemoveFromActiveColliders()
        {
            if(colliders.Contains(this))
                colliders.Remove(this);
        }

        public static List<Collider> ActiveColliders() => colliders;

        public abstract object Clone();
    }
}
