using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Triangulator;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Core.Physics
{
    public class Shape
    {

        private List<Vector2> normals;

        public List<Vector2> Vertices { get; private set; }
        public Shape(List<Vector2> vertices)
        {
            Vertices = new List<Vector2>();
            Vertices.AddRange(vertices); //Copies the vertices;
            CalcNormals();
        }

        /// <summary>
        /// Checks if a shape is colliding with another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="MTV"></param>
        /// <returns></returns>
        public bool IsCollidingWithOtherShape(Shape other, out Vector2? MTV)
        {
            float minOverlap = float.MaxValue;
            Vector2 overlapAxis = Vector2.Zero;

            List<Vector2> myVertices = this.Vertices;
            List<Vector2> otherVertices = other.Vertices;


            List<Vector2> Axes = new List<Vector2>();
            Axes.AddRange(this.GetNormals());
            Axes.AddRange(other.GetNormals());

            Vector2 myCenter = Vector2.Zero;
            Vector2 otherCenter = Vector2.Zero;

            foreach (Vector2 axis in Axes)
            {


                float myMax = float.MinValue;
                float myMin = float.MaxValue;

                float otherMax = float.MinValue;
                float otherMin = float.MaxValue;

                Vector2 sum = Vector2.Zero;
                foreach (Vector2 vertex in myVertices)
                {

                    sum += vertex;

                    float dot = Vector2.Dot(vertex, axis);
                    if (dot > myMax) myMax = dot;
                    if (dot < myMin) myMin = dot;

                }

                myCenter = sum / myVertices.Count;

                sum = Vector2.Zero;
                foreach (Vector2 vertex in otherVertices)
                {
                    sum += vertex;
                    float dot = Vector2.Dot(vertex, axis);
                    if (dot > otherMax) otherMax = dot;
                    if (dot < otherMin) otherMin = dot;
                }

                otherCenter = sum / otherVertices.Count;

                Debugging.Debug.DrawPoint(myCenter, Color.White, 10.0f);
                Debugging.Debug.DrawPoint(otherCenter, Color.Red, 10.0f);

                if (myMax < otherMin || otherMax < myMin)
                {
                    MTV = null;
                    return false;
                }
                else
                {
                    //Find MTV (not the music channel)
                    float thisOverlap = Math.Min(myMax, otherMax) - Math.Max(myMin, otherMin);
                    if (thisOverlap <= minOverlap)
                    {

                        minOverlap = thisOverlap;
                        overlapAxis = axis;
                    }
                }
            }


            Vector2 direction = (myCenter - otherCenter);
            float collidersDot = Vector2.Dot(direction, overlapAxis);

            //Adding a small offset so the two colliders don't keep on colliding after the pushback
            //minOverlap += 0.1f;

            MTV = (overlapAxis * minOverlap);

            if (collidersDot < 0)
            {
                MTV = MTV * -1;
            }
            return true;
        }

        //Triangulating using the Earclipping method
        //https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
        public static List<Shape> GenerateConvexShapesFromVertices(List<Vector2> OriginalVertices)
        {

            if (OriginalVertices.Count < 3) throw new ArgumentException("A shape is composed by at least 3 vertices!");
            List<Shape> shapes = new List<Shape>();
            Vector2[] outVertices = null ;
            int[] outIndices = null;
            Triangulator.Triangulator.Triangulate(OriginalVertices.ToArray(), WindingOrder.Clockwise, out outVertices, out outIndices);

            for(int i = 0; i < outIndices.Length; i+= 3)
            {
                List<Vector2> vertices = new List<Vector2>();

                vertices.Add(outVertices[outIndices[i]]);
                vertices.Add(outVertices[outIndices[i+1]]);
                vertices.Add(outVertices[outIndices[i+2]]);
                Shape newShape = new Shape(vertices);

                shapes.Add(newShape);
            }

            outVertices = null;
            outIndices = null;

            return shapes;
        }

        private void CalcNormals()
        {
            normals = new List<Vector2>(Vertices.Count);
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vector2 vertex = Vertices[i];
                Vector2 nextVertex = (i < Vertices.Count - 1) ? Vertices[i + 1] : Vertices[0];

                Vector2 edge = (nextVertex - vertex);
                Vector2 normal = new Vector2(edge.Y, -edge.X);
                normal.Normalize();

                normals.Add(normal);
            }
        }

        public virtual List<Vector2> GetNormals()
        {
            return normals;
        }

        public static bool ConvexityTest(Shape shape)
        {
            return ConvexityTest(shape.Vertices);
        }

        public static bool ConvexityTest(List<Vector2> points)
        {
            if (points.Count < 3) return false;
            float previousCross = 0.0f;
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 A = points[i];
                Vector2 B = (i < points.Count - 1) ? points[i + 1] : points[0];
                Vector2 C = (i < points.Count - 2) ? points[i + 2] : points[1];

                Vector2 AB = (B - A);
                Vector2 BC = (C - B);

                AB.Normalize();
                BC.Normalize();

                //This isn't actually a cross product as the cross product
                //doesn't exist in 2D
                float cross = Utils.Cross(AB, BC);

                //Do the check only if we're not at the first iteration
                if (i != 0 && Math.Sign(cross) != Math.Sign(previousCross)) return false;

                previousCross = cross;
            }
            return true;
        }

    }
}
