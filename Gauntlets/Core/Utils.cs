using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Core
{
    class Utils
    {

        public static bool IsPointInsideTriangle(List<Vector2> vertices, Vector2 P)
        {
            if (vertices.Count != 3) throw new ArgumentException("The vertices provided do not form a triangle!");
            Vector2 A = vertices[0];
            Vector2 B = vertices[1];
            Vector2 C = vertices[2];

            Vector2 AB = (B - A);
            Vector2 BC = (C - B);
            Vector2 CA = (A - C);

            Vector2 AP = (P - A);
            Vector2 BP = (P - B);
            Vector2 CP = (P - C);

            return (Math.Sign(Cross(AB, AP)) == Math.Sign(Cross(BC, BP))) && (Math.Sign(Cross(BC, BP)) == Math.Sign(Cross(CA, CP)));

        }

        public static float Cross(Vector2 A, Vector2 B)
        {
            return (A.X * B.Y - A.Y * B.X);
        }

        public static bool IsConvex(Vector2 A, Vector2 B, Vector2 C)
        {

            Vector2 AB = (B - A);
            Vector2 BC = (C - B);
            float dot = Vector2.Dot(AB, BC);

            float angle = (float)Math.Acos(dot / (AB.LengthSquared() * BC.LengthSquared()));
            return angle < Math.PI;


        }

        public static float Deg2Rad(float angle)
        {
            return angle * 0.0174533f;
        }

        public static float Rad2Deg(float angle)
        {
            return angle * 57.29580406904963f;
        }

    }
}
