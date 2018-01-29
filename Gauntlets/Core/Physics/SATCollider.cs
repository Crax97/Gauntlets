using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CraxAwesomeEngine.Core.Physics
{
    class SATCollider : Collider
    {

        private List<Vector2> vertices = null;
        private List<Vector2> normals = null;

        public SATCollider(List<Vector2> vertices)
        {
            this.vertices = vertices;
        }

        private void CalculateNormals()
        {
            normals = new List<Vector2>();
            List<Vector2> myVertices = GetColliderVertices();

            for (int i = 0; i < myVertices.Count; i++)
            {
                Vector2 a = myVertices[i];
                Vector2 b = (i < myVertices.Count - 1) ? myVertices[i + 1] : myVertices[0];

                Vector2 diff = (b - a);
                Vector2 axis = new Vector2(diff.Y, -diff.X);
                normals.Add(axis / axis.Length());
            }
            
        }

        public override ColliderType GetColliderType()
        {
            return ColliderType.SAT;
        }

        public override List<Vector2> GetNormals()
        {
            if(normals == null)
                CalculateNormals();
            return normals;
        }

        /// <summary>
        /// Returns the collider vertices translated
        /// by the Entity position
        /// </summary>
        /// <returns></returns>
        public override List<Vector2> GetColliderVertices()
        {
            List<Vector2> translatedVertices = new List<Vector2>();

            foreach(Vector2 vertex in vertices)
            {
                translatedVertices.Add(Owner.Transform.Position + vertex);
            }

            return translatedVertices;
        }

        public override object Clone()
        {
            SATCollider clone = new SATCollider(vertices);
            clone.Initialize(Owner);
            return clone;
        }
    }
}
