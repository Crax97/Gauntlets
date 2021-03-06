﻿using Microsoft.Xna.Framework;
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
    [Serializable]
    public class AABBCollider : Collider
    {

        public Vector2 Extension { get; set; }
        private Vector2 topLeft, topRight;
        private Vector2 bottomLeft, bottomRight;

        public AABBCollider(Vector2 extension) : base()
        {
            IsStatic = true;
            this.Extension = extension;
        }

        public override void Initialize(Entity owner)
        {
            base.Initialize(owner);

            if(Extension == Vector2.Zero)
            {
                //Try creating the extension from the entity's Sprite
                Sprite sprite = owner.GetComponent<Sprite>(true);
                if(sprite != null)
                {
                    Extension = sprite.Size;
                }
            }

        }

        public override void Update(float deltaTime, Entity parent)
        {
            base.Update(deltaTime, parent);

            UpdateBounds(parent.Transform.Position);
        }

        public void UpdateBounds(Vector2 position)
        {

            Vector2 halfExtension = Extension * 0.5f;
            topLeft = position + new Vector2(-halfExtension.X, -halfExtension.Y);
            topRight = position + new Vector2(halfExtension.X, -halfExtension.Y);
            bottomLeft = position + new Vector2(-halfExtension.X, halfExtension.Y);
            bottomRight = position + new Vector2(halfExtension.X, halfExtension.Y);
        }

        public override List<Shape> GetShapes()
        {

            List<Shape> quadShapeList = new List<Shape>();
            List<Vector2> quad = new List<Vector2>();
            quad.Add(topLeft);
            quad.Add(topRight);
            quad.Add(bottomRight);
            quad.Add(bottomLeft);
            QuadShape quadShape = new QuadShape(quad);

            quadShapeList.Add(quadShape);
            return quadShapeList;

        }

        public override ColliderType GetColliderType()
        {
            return ColliderType.AABB;
        }

        public override object Clone()
        {
            AABBCollider clone = new AABBCollider(Extension);
            clone.IsStatic = this.IsStatic;
            return clone;
        }
    }
}
