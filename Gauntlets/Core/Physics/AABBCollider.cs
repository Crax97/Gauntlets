using CraxAwesomeEngine.Content.Scripts.Proxies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Core.Physics
{

    /// <summary>
    /// A Collider based on <see cref="Microsoft.Xna.Framework.Rectangle"/>
    /// </summary>
    public class AABBCollider : Collider
    {

        public Vector2Proxy Extension { get; set; }
        private Vector2Proxy topLeft, topRight;
        private Vector2Proxy bottomLeft, bottomRight;

        public AABBCollider(Vector2Proxy extension)
        {
            this.Extension = extension;
        }

        public override void Update(float deltaTime, Entity parent)
        {
            Vector2 halfExtension = Extension * 0.5f;

            topLeft = parent.Transform.Position + new Vector2Proxy(-halfExtension.X, -halfExtension.Y);
            topRight = parent.Transform.Position + new Vector2Proxy(halfExtension.X, -halfExtension.Y);
            bottomLeft = parent.Transform.Position + new Vector2Proxy(-halfExtension.X, halfExtension.Y);
            bottomRight = parent.Transform.Position + new Vector2Proxy(halfExtension.X, halfExtension.Y);

            base.Update(deltaTime, parent);
        }

        public override List<Vector2Proxy> GetColliderVertices()
        {
            return new List<Vector2Proxy> { topLeft, topRight, bottomLeft, bottomRight };
        }

        public override ColliderType GetColliderType()
        {
            return ColliderType.AABB;
        }

        public override List<Vector2Proxy> GetNormals()
        {
            List<Vector2Proxy> axes = new List<Vector2Proxy>();
            axes.Add(new Vector2Proxy(1, 0));
            axes.Add(new Vector2Proxy(1, 0));
            return axes;
        }

        public override object Clone()
        {
            AABBCollider clone = new AABBCollider(Extension);
            clone.Initialize(Owner);
            return clone;
        }
    }
}
