using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxEngine.Core
{

    /// <summary>
    /// A Collider based on <see cref="Microsoft.Xna.Framework.Rectangle"/>
    /// </summary>
    public class Collider : IComponent
    {

        private static List<Collider> activeColliders = new List<Collider>();

        public Vector2 Extension { get; set; }
        private Vector2 positionLastFrame;
        private Rectangle collisionRectangle;

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
            activeColliders.Remove(this);
        }

        public void Initialize()
        {
            activeColliders.Add(this);
        }

        public void Update(float deltaTime, Entity parent)
        {
            Vector2 parentPosition = parent.Transform.Position;
            Vector2 halfExtension = Extension * 0.5f;

            collisionRectangle = new Rectangle((parent.Transform.Position - Extension * 0.5f).ToPoint(), Extension.ToPoint());

            Vector2 deltaPosition = parent.Transform.Position - positionLastFrame;

            Vector2 pushBack = Vector2.Zero;
            CheckForCollisions(deltaPosition, out pushBack);
            parent.Transform.Translate(-pushBack);
            positionLastFrame = parent.Transform.Position;
        }

        public void CheckForCollisions(Vector2 deltaPosition, out Vector2 pushBack)
        {
            pushBack = Vector2.Zero;
            foreach (Collider other in activeColliders)
            {
                if (other != this && collisionRectangle.Intersects(other.collisionRectangle))
                {
                    //Are we moving right or left?
                    if (deltaPosition.X > 0)
                    {
                        pushBack.X = collisionRectangle.Right - other.collisionRectangle.Left;
                    }
                    else if (deltaPosition.X < 0)
                    {
                        pushBack.X = collisionRectangle.Left - other.collisionRectangle.Right;
                    }

                    //Are we moving up or down?
                    if (deltaPosition.Y < 0)
                    {
                        pushBack.Y = collisionRectangle.Top - other.collisionRectangle.Bottom;
                    }
                    else if (deltaPosition.Y > 0)
                    {
                        pushBack.Y = collisionRectangle.Bottom - other.collisionRectangle.Top;
                    }

                }
            }
        }
    }
}
