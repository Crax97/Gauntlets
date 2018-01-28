using CraxAwesomeEngine.Content.Scripts.Proxies;
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

        private Vector2 positionPreviousFrame;
        public Entity Owner { get; private set; } = null;
        public bool IsStatic { get; set; } = false;
        
        public Action<Collider> OnCollision { get; set; } = null;

        public abstract List<Vector2Proxy> GetNormals();

        private List<Vector2Proxy> GetAxes(Collider other)
        {
            List<Vector2Proxy> axes = new List<Vector2Proxy>();
            if ((GetColliderType() == ColliderType.AABB && other.GetColliderType() == ColliderType.AABB))
            {
                axes.Add(new Vector2Proxy(1, 0));
                axes.Add(new Vector2Proxy(0, 1));
            }
            else
            {
                axes.AddRange(this.GetNormals());
                axes.AddRange(other.GetNormals());
            }
            return axes;
        }

        /// <summary>
        /// Checks if two Colliders are colliding using
        /// the Separating Axis Theorem
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CheckForCollisionSAT(Collider other, out Vector2Proxy MTV)
        {
            List<Vector2Proxy> Axes = GetAxes(other);

            float minOverlap = float.MaxValue;
            Vector2Proxy overlapAxis = Vector2Proxy.Zero;

            foreach(Vector2Proxy axis in Axes)
            {
                List<Vector2Proxy> myVertices = GetColliderVertices();
                List<Vector2Proxy> otherVertices = other.GetColliderVertices();

                float myMax = float.MinValue;
                float myMin = float.MaxValue;

                float otherMax = float.MinValue;
                float otherMin = float.MaxValue;

                foreach (Vector2Proxy vertex in myVertices)
                {
                    float dot = Vector2.Dot(vertex, axis);
                    if (dot > myMax) myMax = dot;
                    if (dot < myMin) myMin = dot;
                }

                foreach (Vector2Proxy vertex in otherVertices)
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

            Vector2Proxy direction = (this.Owner.Transform.Position - other.Owner.Transform.Position);
            float collidersDot = Vector2Proxy.Dot(direction, overlapAxis);

            //Adding a small offset so the two colliders don't keep on colliding after the pushback
            //minOverlap += 0.1f;

            MTV = overlapAxis * minOverlap;

            if (collidersDot < 0)
             {
                 MTV = MTV * -1;
             }

            return true;
        }
        
        public abstract List<Vector2Proxy> GetColliderVertices();
        public abstract ColliderType GetColliderType();

        public void Initialize(Entity owner)
        {
            colliders.Add(this);
            this.Owner = owner;
        }
       

        public virtual void Update(float deltaTime, Entity parent)
        {
            positionPreviousFrame = parent.Transform.Position;
        }

        public static void CalculateCollisions()
        {
            foreach (Collider collider in colliders) {
                foreach (Collider other in colliders)
                {
                    if (other != collider)
                    {
                        Vector2Proxy pushback;
                        if (collider.CheckForCollisionSAT(other, out pushback))
                        {
                            if (!collider.IsStatic) collider.Owner.Transform.Translate(pushback);
                            collider.OnCollision?.Invoke(other);
                        }
                    }
                }
            }
        }

        public void Destroy()
        {
            colliders.Remove(this);
        }

        public abstract object Clone();
    }
}
