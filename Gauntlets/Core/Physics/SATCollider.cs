using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CraxAwesomeEngine.Content.Scripts.Proxies;

namespace CraxAwesomeEngine.Core.Physics
{
    class SATCollider : Collider
    {

        private List<Vector2Proxy> vertices = null;
        private List<Vector2Proxy> normals = null;

        public SATCollider(List<Vector2Proxy> vertices)
        {
            this.vertices = vertices;
        }

        private void CalculateNormals()
        {
            normals = new List<Vector2Proxy>();
            List<Vector2Proxy> myVertices = GetColliderVertices();

            for (int i = 0; i < myVertices.Count; i++)
            {
                Vector2Proxy a = myVertices[i];
                Vector2Proxy b = (i < myVertices.Count - 1) ? myVertices[i + 1] : myVertices[0];

                Vector2Proxy diff = (b - a);
                Vector2Proxy axis = new Vector2Proxy(diff.Y, -diff.X);
                normals.Add(axis / axis.Length());
            }
            
        }

        public override ColliderType GetColliderType()
        {
            return ColliderType.SAT;
        }

        public override List<Vector2Proxy> GetNormals()
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
        public override List<Vector2Proxy> GetColliderVertices()
        {
            List<Vector2Proxy> translatedVertices = new List<Vector2Proxy>();

            foreach(Vector2Proxy vertex in vertices)
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
