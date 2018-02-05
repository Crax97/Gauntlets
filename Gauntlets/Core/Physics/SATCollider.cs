using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CraxAwesomeEngine.Core.Physics
{
    class SATCollider : Collider
    {
        
        private List<Shape> myShapes;

        private SATCollider() : base()
        {

        }

        public SATCollider(List<Vector2> vertices) : base()
        {
            IsStatic = true;
            myShapes = Shape.GenerateConvexShapesFromVertices(vertices);
        }

        public override ColliderType GetColliderType()
        {
            return ColliderType.SAT;
        }

        public override void Update(float deltaTime, Entity parent)
        {
            base.Update(deltaTime, parent);

        }

        /// <summary>
        /// Returns the collider vertices translated
        /// by the Entity position
        /// </summary>
        /// <returns></returns>
        public override List<Shape> GetShapes()
        {
            List<Shape> translatedShapes = new List<Shape>(myShapes.Count);
            foreach(Shape shape in myShapes)
            {
                List<Vector2> translatedVertices = new List<Vector2>(shape.Vertices.Count);
                foreach(Vector2 vertex in shape.Vertices)
                {
                    Vector2 translatedVertex = vertex + Owner.Transform.Position;
                    translatedVertices.Add(translatedVertex);
                }
                translatedShapes.Add(new Shape(translatedVertices));
            }

            return translatedShapes;
        }

        public override object Clone()
        {
            SATCollider clone = new SATCollider();
            clone.myShapes = new List<Shape>(myShapes.Count);
            clone.myShapes.AddRange(this.myShapes);
            return clone;
        }
    }
}
