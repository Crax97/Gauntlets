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

        public Vector2 Extension { get; set; }
        private Vector2 topLeft, topRight;
        private Vector2 bottomLeft, bottomRight;

        public AABBCollider(Vector2 extension)
        {
            this.Extension = extension;
        }

        public override void Update(float deltaTime, Entity parent)
        {
            Vector2 halfExtension = Extension * 0.5f;

            topLeft = parent.Transform.Position + new Vector2(-halfExtension.X, -halfExtension.Y);
            topRight = parent.Transform.Position + new Vector2(halfExtension.X, -halfExtension.Y);
            bottomLeft = parent.Transform.Position + new Vector2(-halfExtension.X, halfExtension.Y);
            bottomRight = parent.Transform.Position + new Vector2(halfExtension.X, halfExtension.Y);

            base.Update(deltaTime, parent);
        }

        public override List<Vector2> GetColliderVertices()
        {
            return new List<Vector2> { topLeft, topRight, bottomLeft, bottomRight };
        }

        public override ColliderType GetColliderType()
        {
            return ColliderType.AABB;
        }

        public override List<Vector2> GetNormals()
        {
            List<Vector2> axes = new List<Vector2>();
            axes.Add(new Vector2(1, 0));
            axes.Add(new Vector2(1, 0));
            return axes;
        }
        
    }
}
