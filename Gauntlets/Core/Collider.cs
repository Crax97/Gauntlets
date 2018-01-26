using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxEngine.Core
{
    class Collider : IComponent
    {
        private Rectangle collisionRectangle;
        public Vector2 Extension {get; set;}

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
            collisionRectangle = new Rectangle((parent.Transform.PositionInCameraSpace - Extension * 0.5f).ToPoint(), Extension.ToPoint());
        }

        public bool IsCollidingWith(Collider other)
        {
            return (collisionRectangle.Intersects(other.collisionRectangle));
        }

        public bool HasPointInside(Vector2 point)
        {
            return (collisionRectangle.Contains(point));
        }
    }
}
