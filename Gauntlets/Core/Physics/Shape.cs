using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Triangulator;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Core.Physics
{
    class Shape
    {

        public List<Vector2> Vertices { get; private set; }
        public Shape(List<Vector2> vertices)
        {
            Vertices = new List<Vector2>();
            Vertices.AddRange(vertices); //Copies the vertices;
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

                shapes.Add(new Shape(vertices));
            }

            outVertices = null;
            outIndices = null;

            return shapes;
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
