using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxEngine.Core
{
    public class Collider : IComponent
    {
        private Rectangle collisionRectangle;
        public Vector2 Extension {get; set;}

        private Vector2 topLeft;
        private Vector2 topRight;
        private Vector2 bottomLeft;
        private Vector2 bottomRight;

        public Collider(Vector2 extension)
        {
            this.Extension = extension;
        }

        public object Clone()
        {
            return new Collider(Extension);
        }

        public void Destroy()
        {
        }

        public void Initialize()
        {
        }

        public void Update(float deltaTime, Entity parent)
        {
            Vector2 parentPosition = parent.Transform.PositionInCameraSpace;
            Vector2 halfExtension = Extension * 0.5f;

            topLeft = parentPosition + new Vector2(-halfExtension.X, -halfExtension.Y);
            topRight = parentPosition + new Vector2(halfExtension.X, -halfExtension.Y);
            bottomLeft = parentPosition + new Vector2(-halfExtension.X, halfExtension.Y);
            bottomRight = parentPosition + new Vector2(halfExtension.X, halfExtension.Y);

            collisionRectangle = new Rectangle((parent.Transform.PositionInCameraSpace - Extension * 0.5f).ToPoint(), Extension.ToPoint());
        }

        public bool IsCollidingWith(Collider other, Vector2 offset)
        {
            return (other.HasPointInside(topLeft + offset) ||
                other.HasPointInside(topRight + offset) ||
                other.HasPointInside(bottomLeft + offset) ||
                other.HasPointInside(bottomRight + offset)); 
        }

        private bool lineColliding(Collider other, Vector2 begin, Vector2 end, Vector2 offset)
        {
            return (other.HasPointInside(begin + offset) || other.HasPointInside(end + offset));
        }

        public bool IsRightColliding(Collider other, Vector2 offset)
        {
            return lineColliding(other, topRight, bottomRight, offset);
        }

        public bool IsLeftColliding(Collider other, Vector2 offset)
        {
            return lineColliding(other, topLeft, bottomLeft, offset);
        }
        public bool IsUpColliding(Collider other, Vector2 offset)
        {
            return lineColliding(other, topLeft, topRight, offset);
        }
        public bool IsDownColliding(Collider other, Vector2 offset)
        {
            return lineColliding(other, bottomLeft, bottomRight, offset);
        }

        public bool HasPointInside(Vector2 point)
        {
            return (collisionRectangle.Contains(point));
        }
        
    }
        
}
