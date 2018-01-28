using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Content.Scripts.Proxies
{
    /// <summary>
    /// A Proxy class for <see cref="Microsoft.Xna.Framework.Vector2"/>,
    /// to be passed to lua scripts.
    /// https://msdn.microsoft.com/en-us/library/microsoft.xna.framework.vector2_members.aspx
    /// </summary>
    [MoonSharpUserData]
    public class Vector2Proxy
    {

        private Vector2 vec;

        public Vector2Proxy(float X, float Y)
        {
            vec = new Vector2(X, Y);
        }

        public Vector2Proxy(Vector2 carried)
        {
            vec = carried;
        }

        public float X
        {
            get
            {
                return vec.X;
            }
            set
            {
                vec.X = value;
            }
        }

        public float Y
        {
            get
            {
                return vec.Y;
            }
            set
            {
                vec.Y = value;
            }
        }

        public float Length()
        {
            return vec.Length();
        }

        public float LengthSquared()
        {
            return vec.LengthSquared();
        }

        public void Normalize()
        {
            vec.Normalize();
        }

        public Vector2Proxy Negated()
        {
            return new Vector2Proxy(Vector2.Negate(vec));
        }

        public static implicit operator Vector2(Vector2Proxy vec)
        {
            return vec.vec;
        }

        public override string ToString()
        {
            return vec.ToString();
        }

        public override bool Equals(object obj)
        {
            return vec.Equals(obj);
        }

        public override int GetHashCode()
        {
            return vec.GetHashCode();
        }

        public static Vector2Proxy operator +(Vector2Proxy a, Vector2Proxy b)
        {
            return Add(a, b);
        }

        public static Vector2Proxy operator-(Vector2Proxy a, Vector2Proxy b)
        {
            return new Vector2Proxy(a.vec - b.vec);
        }

        public static Vector2Proxy operator*(Vector2Proxy a, float scalar)
        {
            return Multiply(a, scalar);
        }

        public static Vector2Proxy operator/(Vector2Proxy a, float scalar)
        {
            return Divide(a, scalar);
        }

        public static Vector2Proxy Add(Vector2Proxy a, Vector2Proxy b)
        {
            return new Vector2Proxy(a.vec + b.vec);
        }

        public static Vector2Proxy Divide(Vector2Proxy vecProxy, float scalar)
        {
            return new Vector2Proxy(Vector2.Divide(vecProxy.vec, scalar));
        }

        public static Vector2Proxy Multiply(Vector2Proxy vecProxy, float scalar)
        {
            return new Vector2Proxy(Vector2.Multiply(vecProxy.vec, scalar));
        }

        public static Vector2Proxy One
        {
            get
            {
                return new Vector2Proxy(1, 1);
            }
        }

        public static Vector2Proxy Zero
        {
            get
            {
                return new Vector2Proxy(0, 0);
            }
        }

        public static float Dot(Vector2Proxy a, Vector2Proxy b)
        {
            return Vector2.Dot(a.vec, b.vec);
        }

        public static float Distance(Vector2Proxy a, Vector2Proxy b)
        {
            return Vector2.Distance(a.vec, b.vec);
        }

        public static float DistanceSquared(Vector2Proxy a, Vector2Proxy b)
        {
            return Vector2.DistanceSquared(a.vec, b.vec);
        }

        public static Vector2Proxy Lerp(Vector2Proxy a, Vector2Proxy b, float time)
        {
            return new Vector2Proxy(Vector2.Lerp(a, b, time));
        }

        public static Vector2Proxy Slerp(Vector2Proxy a, Vector2Proxy b, float time)
        {
            return new Vector2Proxy(Vector2.SmoothStep(a, b, time));
        }

    }
}
