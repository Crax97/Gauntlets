using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxEngine.Core
{
    interface ICollider
    {

        bool HasPointInside(Vector2 point);
        bool IsCollidingWith(ICollider other);
    }
}
