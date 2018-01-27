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
        /// <summary>
        /// Gets the axis of the edges of two shapes
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /*private List<Vector2> GetAxis(Collider other)
        {
            List<Vector2> normals = new List<Vector2>();
            List<Vector2> myVertices = GetColliderVertices();
            List<Vector2> otherVertices = other.GetColliderVertices();

            for (int i = 0; i < myVertices.Count; i++)
            {
                Vector2 a = myVertices[i];
                Vector2 b = (i < myVertices.Count - 1) ? myVertices[i + 1] : myVertices[0];

                Vector2 diff = (b - a);
                normals.Add(new Vector2(diff.Y, -diff.X));
            }

            
            for (int i = 0; i < otherVertices.Count; i++)
            {
                Vector2 a = otherVertices[i];
                Vector2 b = (i < otherVertices.Count - 1) ? otherVertices[i + 1] : otherVertices[0];

                Vector2 diff = (b - a);
                normals.Add(new Vector2(diff.Y, -diff.X));
            }

            return normals;
        }*/

        public abstract List<Vector2> GetNormals();

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
                axes.Concat<Vector2>(this.GetNormals());
                axes.Concat<Vector2>(other.GetNormals());
            }
            return axes;
        }

        /// <summary>
        /// Checks if two Colliders are colliding using
        /// the Separating Axis Theorem
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CheckForCollisionSAT(Collider other, out Vector2? MTV)
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

            Vector2 distance = (this.Owner.Transform.Position - other.Owner.Transform.Position);
            float collidersDot = Vector2.Dot(distance, overlapAxis);
            MTV = overlapAxis * minOverlap;

            if (collidersDot < 0)
            {
                MTV = MTV * -1;
            }

            return true;
        }
        
        protected abstract List<Vector2> GetColliderVertices();
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
                        Vector2? pushback;
                        collider.CheckForCollisionSAT(other, out pushback);
                        if (pushback != null && !collider.IsStatic) 
                        {
                            collider.Owner.Transform.Translate(pushback.Value);
                        }
                    }
                }
            }
        }

        public void Destroy()
        {
            colliders.Remove(this);
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
