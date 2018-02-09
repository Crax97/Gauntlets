using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CraxAwesomeEngine.Core.Physics
{
    [Serializable]
    class QuadShape : Shape
    {
        public QuadShape(List<Vector2> vertices) : base(vertices)
        {
        }

        public override List<Vector2> GetNormals()
        {
            List<Vector2> axes = new List<Vector2>(2);
            axes.Add(new Vector2(1, 0));
            axes.Add(new Vector2(0, 1));
            return axes;
        }

    }
}
