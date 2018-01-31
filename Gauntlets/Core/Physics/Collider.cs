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
        private static List<Collider> colliders = new List<Collider>();

        public Entity Owner { get; private set; } = null;
        public bool IsStatic { get; set; } = false;
        public Action<Collider> OnCollision { get; set; } = null;
        public bool isColliding = false;
        public abstract List<Vector2> GetNormals();
        public abstract List<Vector2> GetColliderVertices();
        public abstract ColliderType GetColliderType();

        private Vector2 positionPreviousFrame;
        private List<Vector2> GetAxes(Collider other)
        {
            List<Vector2> axes = new List<Vector2>();
            if ((GetColliderType() == ColliderType.AABB && other.GetColliderType() == ColliderType.AABB))
            {
                axes.Add(new Vector2(1, 0));
                axes.Add(new Vector2(0, 1));
            }
            else
            {
                axes.AddRange(this.GetNormals());
                axes.AddRange(other.GetNormals());
            }
            return axes;
        }

        public static void CalculateCollisions()
        {
            foreach (Collider collider in colliders)
            {

                collider.isColliding = false;

                foreach (Collider other in colliders)
                {
                    if (other != collider)
                    {
                        Vector2? pushback;
                        collider.SATIsCollidingWith(other, out pushback);
                        if (pushback != null)
                        {
                            if (!collider.IsStatic) collider.Owner.Transform.Translate(pushback.Value);
                            collider.OnCollision?.Invoke(other);
                            collider.isColliding = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if two Colliders are colliding using
        /// the Separating Axis Theorem
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SATIsCollidingWith(Collider other, out Vector2? MTV)
        {
            List<Vector2> Axes = GetAxes(other);

            float minOverlap = float.MaxValue;
            Vector2 overlapAxis = Vector2.Zero;

            foreach(Vector2 axis in Axes)
            {
                List<Vector2> myVertices = GetColliderVertices();
                List<Vector2> otherVertices = other.GetColliderVertices();

                float myMax = float.MinValue;
                float myMin = float.MaxValue;

                float otherMax = float.MinValue;
                float otherMin = float.MaxValue;

                foreach (Vector2 vertex in myVertices)
                {
                    float dot = Vector2.Dot(vertex, axis);
                    if (dot > myMax) myMax = dot;
                    if (dot < myMin) myMin = dot;
                }

                foreach (Vector2 vertex in otherVertices)
                {
                    float dot = Vector2.Dot(vertex, axis);
                    if (dot > otherMax) otherMax = dot;
                    if (dot < otherMin) otherMin = dot;
                }

                if (myMax < otherMin || otherMax < myMin)
                {
                    MTV = null;
                    return false;
                }
                else
                {
                    //Find MTV (not the music channel)
                    float thisOverlap = Math.Min(myMax, otherMax) - Math.Max(myMin, otherMin);
                    if (thisOverlap < minOverlap)
                    {
                        minOverlap = thisOverlap;
                        overlapAxis = axis;
                    }
                }

            }

            Vector2 direction = (this.Owner.Transform.Position - other.Owner.Transform.Position);
            float collidersDot = Vector2.Dot(direction, overlapAxis);

            //Adding a small offset so the two colliders don't keep on colliding after the pushback
            //minOverlap += 0.1f;

            MTV = overlapAxis * minOverlap;

            if (collidersDot < 0)
             {
                 MTV = MTV * -1;
             }

            return true;
        }


        public void Initialize(Entity owner)
        {
            colliders.Add(this);
            this.Owner = owner;
        }
       

        public virtual void Update(float deltaTime, Entity parent)
        {
            positionPreviousFrame = parent.Transform.Position;
        }


        public void Destroy()
        {
            colliders.Remove(this);
        }

        public bool IsColliding()
        {
            return isColliding;
        }

        public abstract object Clone();
    }
}
